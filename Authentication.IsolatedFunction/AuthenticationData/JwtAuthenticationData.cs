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

using System.Security.Claims;

#endregion

namespace Sidub.Platform.Authentication.IsolatedFunction.AuthenticationData
{

    /// <summary>
    /// Represents the data for JWT authentication.
    /// </summary>
    public class JwtAuthenticationData : IAuthenticationData
    {

        #region Public properties

        /// <summary>
        /// Gets the claims principal associated with the authentication data.
        /// </summary>
        public ClaimsPrincipal? Principal { get; }

        /// <summary>
        /// Gets the access token associated with the authentication data.
        /// </summary>
        internal string AccessToken { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="JwtAuthenticationData"/> class.
        /// </summary>
        /// <param name="principal">The claims principal.</param>
        /// <param name="accessToken">The access token.</param>
        public JwtAuthenticationData(ClaimsPrincipal? principal, string accessToken)
        {
            Principal = principal;
            AccessToken = accessToken;
        }

        #endregion

    }

}
