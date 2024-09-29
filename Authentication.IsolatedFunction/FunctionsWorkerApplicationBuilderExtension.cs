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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

#endregion

namespace Sidub.Platform.Authentication.IsolatedFunction
{

    /// <summary>
    /// Extension methods for configuring authentication in Functions Worker Application Builder.
    /// </summary>
    public static class FunctionsWorkerApplicationBuilderExtension
    {

        #region Public static methods

        /// <summary>
        /// Adds Sidub authentication for isolated function.
        /// </summary>
        /// <param name="builder">The Functions Worker Application Builder.</param>
        /// <returns>The Functions Worker Application Builder.</returns>
        public static IFunctionsWorkerApplicationBuilder AddSidubAuthenticationForIsolatedFunction(this IFunctionsWorkerApplicationBuilder builder)
        {
            builder.UseMiddleware<AuthenticationMiddleware>();

            builder.Services.AddOptions<TokenValidationParameters>().Configure<IOptionsSnapshot<AuthenticationOptions>>((options, authOptions) =>
            {
                options.ValidAudience = authOptions.Value.ValidAudience;
                options.ValidIssuer = authOptions.Value.ValidIssuer;
            });

            return builder;
        }

        #endregion

    }

}
