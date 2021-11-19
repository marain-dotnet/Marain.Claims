// <copyright file="Startup.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

[assembly: Microsoft.Azure.Functions.Extensions.DependencyInjection.FunctionsStartup(typeof(Marain.Claims.Functions.Startup))]

namespace Marain.Claims.Functions
{
    using Microsoft.Azure.Functions.Extensions.DependencyInjection;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Startup code for the Function.
    /// </summary>
    public class Startup : FunctionsStartup
    {
        /// <inheritdoc/>
        public override void Configure(IFunctionsHostBuilder builder)
        {
            IServiceCollection services = builder.Services;

            IConfiguration root = builder.GetContext().Configuration;
            string serviceTenantIdOverride = root?["MarainServiceConfiguration:ServiceTenantIdOverride"];
            if (!string.IsNullOrWhiteSpace(serviceTenantIdOverride))
            {
                root["MarainServiceConfiguration:ServiceTenantId"] = serviceTenantIdOverride;
            }

            services.AddApplicationInsightsInstrumentationTelemetry();
            services.AddLogging();

            services.AddTenantedClaimsApiWithOpenApiActionResultHosting(
                root,
                config => config.Documents.AddSwaggerEndpoint());
        }
    }
}
