// <copyright file="Host.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

namespace Endjin.Claims.Functions
{
    using System;
    using System.Threading.Tasks;
    using Endjin.Claims.ClaimsProviderStrategies;
    using Endjin.Claims.Client;
    using Endjin.Claims.Serialization.Json.Installers;
    using Endjin.Logging;
    using Endjin.OpenApi;
    using Endjin.OpenApi.AccessControlPolicies;
    using Endjin.OpenApi.Claims;
    using Endjin.OpenApi.Tenancy.TenantProviderStrategies;
    using Endjin.Telemetry;
    using Endjin.Tenancy;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Serilog;
    using Functions = Endjin.Functions;

    /// <summary>
    /// The host for the functions app.
    /// </summary>
    public static class Host
    {
        /// <summary>
        /// The entry point for this functions app. Routes requests to
        /// <see cref="IOpenApiService"/> instances based on the service yaml file.
        /// </summary>
        /// <param name="request">
        /// The incoming request.
        /// </param>
        /// <param name="context">
        /// The request context.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that completes when the request has been processed.
        /// </returns>
        [FunctionName("ClaimsHost-OpenApiHostRoot")]
        public static async Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", Route = "{*path}")]
            HttpRequest request,
            ExecutionContext context)
        {
            Initializer.Initialize(context);

            return await request.HandleRequestAsync().ConfigureAwait(false);
        }
    }
}
