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

namespace Sidub.Platform.Authentication.IsolatedFunction
{

    /// <summary>
    /// Represents the options for authentication.
    /// </summary>
    public class AuthenticationOptions
    {

        #region Public properties

        /// <summary>
        /// Gets or sets the valid audience for the authentication.
        /// </summary>
        public string? ValidAudience { get; set; }

        /// <summary>
        /// Gets or sets the valid issuer for the authentication.
        /// </summary>
        public string? ValidIssuer { get; set; }

        #endregion

    }

}
