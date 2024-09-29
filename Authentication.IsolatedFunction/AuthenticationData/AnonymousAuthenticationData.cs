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
    /// Represents anonymous authentication data.
    /// </summary>
    public class AnonymousAuthenticationData : IAuthenticationData
    {

        #region Public properties

        /// <summary>
        /// Gets the claims principal associated with the anonymous authentication data.
        /// </summary>
        public ClaimsPrincipal Principal { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AnonymousAuthenticationData"/> class.
        /// </summary>
        public AnonymousAuthenticationData()
        {
            Principal = new ClaimsPrincipal();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Initializes a new instance of the <see cref="AnonymousAuthenticationData"/> class with the specified email and display name.
        /// </summary>
        /// <param name="email">The email associated with the anonymous authentication data.</param>
        /// <param name="displayName">The display name associated with the anonymous authentication data.</param>
        public AnonymousAuthenticationData(string email, string displayName)
        {
            Principal = new ClaimsPrincipal();
            Principal.AddIdentity(new ClaimsIdentity(new[]
            {
                    new Claim("emails", email),
                    new Claim("name", displayName)
                }));
        }

        #endregion

    }

}
