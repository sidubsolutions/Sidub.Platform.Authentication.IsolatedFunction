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
using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using System.Reflection;

#endregion

namespace Sidub.Platform.Authentication.IsolatedFunction
{

    /// <summary>
    /// Extension methods for the FunctionContext class.
    /// </summary>
    public static class FunctionContextExtension
    {

        #region Public static methods

        /// <summary>
        /// Gets the MethodInfo of the target function.
        /// </summary>
        /// <param name="context">The FunctionContext instance.</param>
        /// <returns>The MethodInfo of the target function.</returns>
        public static MethodInfo GetTargetFunctionMethod(this FunctionContext context)
        {
            var entryPoint = context.FunctionDefinition.EntryPoint;

            var assemblyPath = context.FunctionDefinition.PathToAssembly;
            var assembly = Assembly.LoadFrom(assemblyPath);
            var typeName = entryPoint.Substring(0, entryPoint.LastIndexOf('.'));
            var type = assembly.GetType(typeName);
            var methodName = entryPoint.Substring(entryPoint.LastIndexOf('.') + 1);
            var method = type.GetMethod(methodName);
            return method;
        }

        /// <summary>
        /// Sets the HTTP response status code and response value.
        /// </summary>
        /// <param name="context">The FunctionContext instance.</param>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <param name="responseValue">The response value.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task SetHttpResponseStatusCode(this FunctionContext context, HttpStatusCode statusCode, string responseValue = "")
        {
            var httpReqData = await context.GetHttpRequestDataAsync();

            if (httpReqData != null)
            {
                var newHttpResponse = httpReqData.CreateResponse(statusCode);
                await newHttpResponse.WriteStringAsync(responseValue);

                context.GetInvocationResult().Value = newHttpResponse;
            }
        }

        /// <summary>
        /// Sets the HTTP response status code and response value as JSON.
        /// </summary>
        /// <param name="context">The FunctionContext instance.</param>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <param name="message">The message to be included in the JSON response.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task SetHttpJsonResponseStatusCode(this FunctionContext context, HttpStatusCode statusCode, string message = "")
        {
            var httpReqData = await context.GetHttpRequestDataAsync();

            if (httpReqData != null)
            {
                var newHttpResponse = httpReqData.CreateResponse(statusCode);

                // https://github.com/Azure/azure-functions-dotnet-worker/issues/776
                await newHttpResponse.WriteAsJsonAsync(new { Message = message }, newHttpResponse.StatusCode);
                context.GetInvocationResult().Value = newHttpResponse;
            }
        }

        #endregion

    }

}
