/*
 * Sidub Platform - Authentication - Isolated Function
 * Copyright (C) 2024 Sidub Inc.
 * All rights reserved.
 *
 * This file is part of Sidub Platform - Authentication - Isolated Function (the "Product").
 *
 * The Product is dual-licensed under:
 * 1. The GNU Affero General Public License version 3 (AGPLv3)
 * 2. Sidub Inc.'s Proprietary Software License Agreement (PSLA)
 *
 * You may choose to use, redistribute, and/or modify the Product under
 * the terms of either license.
 *
 * The Product is provided "AS IS" and "AS AVAILABLE," without any
 * warranties or conditions of any kind, either express or implied, including
 * but not limited to implied warranties or conditions of merchantability and
 * fitness for a particular purpose. See the applicable license for more
 * details.
 *
 * See the LICENSE.txt file for detailed license terms and conditions or
 * visit https://sidub.ca/licensing for a copy of the license texts.
 */

#region Imports

using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Sidub.Platform.Authentication.IsolatedFunction.AuthenticationData;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;

#endregion

namespace Sidub.Platform.Authentication.IsolatedFunction
{

    /// <summary>
    /// Middleware for authentication in Azure Functions.
    /// </summary>
    public class AuthenticationMiddleware : IFunctionsWorkerMiddleware
    {

        #region Member variables

        private readonly JwtSecurityTokenHandler _tokenValidator;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly ConfigurationManager<OpenIdConnectConfiguration> _configurationManager;
        private readonly ILogger<AuthenticationMiddleware> _logger;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationMiddleware"/> class.
        /// </summary>
        /// <param name="tokenValidationOptions">The token validation options.</param>
        /// <param name="logger">The logger.</param>
        public AuthenticationMiddleware(IOptionsSnapshot<TokenValidationParameters> tokenValidationOptions, ILogger<AuthenticationMiddleware> logger)
        {
            _tokenValidator = new JwtSecurityTokenHandler();
            _tokenValidationParameters = tokenValidationOptions.Value;
            _logger = logger;

            if (string.IsNullOrEmpty(_tokenValidationParameters.ValidIssuer))
                throw new Exception("TokenValidationParameters options class must have 'ValidIssuer' populated.");

            var baseUri = new Uri(_tokenValidationParameters.ValidIssuer.TrimEnd('/') + "/");
            var fullUri = new Uri(baseUri, ".well-known/openid-configuration");

            _configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                fullUri.ToString(),
                new OpenIdConnectConfigurationRetriever());
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Invokes the authentication middleware.
        /// </summary>
        /// <param name="context">The function context.</param>
        /// <param name="next">The next middleware delegate.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task Invoke(
            FunctionContext context,
            FunctionExecutionDelegate next)
        {
            var isAnonymousAllowed = IsAnonymousAllowed(context);

            if (!TryGetTokenFromHeaders(context, out var token))
            {
                if (!isAnonymousAllowed)
                {
                    _logger.LogError($"Authentication token was not found and target method '{context.FunctionDefinition.Name}' does not support anonymous access.");
                    await context.SetHttpResponseStatusCode(HttpStatusCode.Unauthorized);
                    return;
                }

                context.Features.Set<IAuthenticationData>(new AnonymousAuthenticationData());
                await next(context);
                return;
            }

            if (!_tokenValidator.CanReadToken(token))
            {
                _logger.LogError("Authentication token was found but did not pass validation. Returning unauthorized.");
                await context.SetHttpResponseStatusCode(HttpStatusCode.Unauthorized);
                return;
            }

            var validationParameters = _tokenValidationParameters.Clone();
            var openIdConfig = await _configurationManager.GetConfigurationAsync();
            validationParameters.ValidIssuer = openIdConfig.Issuer;
            validationParameters.IssuerSigningKeys = openIdConfig.SigningKeys;

            try
            {
                // validate token
                bool validationFailed = false;
                ClaimsPrincipal? principal = null;
                try
                {
                    principal = _tokenValidator.ValidateToken(
                        token, validationParameters, out _);

                    context.Features.Set<IAuthenticationData>(new JwtAuthenticationData(principal, token));
                }
                catch (Exception ex)
                {
                    validationFailed = true;
                }

                if (validationFailed)
                {
                    if (!isAnonymousAllowed)
                        throw new Exception("Authentication token was found but did not pass validation.");

                    var tokenRead = _tokenValidator.ReadJwtToken(token);

                    var email = tokenRead.Claims.SingleOrDefault(x => x.Type == "emails")?.Value;
                    var displayName = tokenRead.Claims.SingleOrDefault(x => x.Type == "name")?.Value;

                    context.Features.Set<IAuthenticationData>(new AnonymousAuthenticationData(email, displayName));
                }

                await next(context);
            }
            catch (SecurityTokenException ex)
            {
                // TODO - considerations here, might be exposing too much details in ex.Message!!!
                _logger.LogError(ex, "Exception was encountered validating authorization.");
                await context.SetHttpResponseStatusCode(HttpStatusCode.Unauthorized);
                return;
            }
        }

        #endregion

        #region Private methods

        private static bool IsAnonymousAllowed(FunctionContext context)
        {
            var method = context.GetTargetFunctionMethod();

            return method.GetCustomAttributes<AllowAnonymousAttribute>().Any()
                || (method.DeclaringType?.GetCustomAttributes<AllowAnonymousAttribute>().Any()
                ?? false);
        }

        private static bool TryGetTokenFromHeaders(FunctionContext context, out string token)
        {
            token = string.Empty;

            if (!context.BindingContext.BindingData.TryGetValue("Headers", out var headersObj))
            {
                return false;
            }

            if (headersObj is not string headersStr)
            {
                return false;
            }

            // Deserialize headers from JSON
            var headers = JsonSerializer.Deserialize<Dictionary<string, string>>(headersStr);
            var normalizedKeyHeaders = headers.ToDictionary(h => h.Key.ToLowerInvariant(), h => h.Value);
            if (!normalizedKeyHeaders.TryGetValue("authorization", out var authHeaderValue))
            {
                // No Authorization header present
                return false;
            }

            if (!authHeaderValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                // Scheme is not Bearer
                return false;
            }

            token = authHeaderValue.Substring("Bearer ".Length).Trim();
            return true;
        }

        #endregion

    }

}
