// <copyright file="Startup.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

[assembly: Microsoft.Azure.Functions.Extensions.DependencyInjection.FunctionsStartup(typeof(Marain.Claims.Functions.Startup))]

namespace Marain.Claims.Functions
{
    using Marain.Claims.OpenApi;

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
            string serviceTenantIdOverride = root["MarainServiceConfiguration:ServiceTenantIdOverride"];
            if (!string.IsNullOrWhiteSpace(serviceTenantIdOverride))
            {
                root["MarainServiceConfiguration:ServiceTenantId"] = serviceTenantIdOverride;
            }

            ConfigureInstrumentation(services);
            ConfigureServiceIdentity(services, root);
            ConfigureAuthenticationStrategies(services);

            services.AddTenantedClaimsStoreOnAzureBlobStorage();
            services.AddTenantedClaimsApiWithOpenApiActionResultHosting(
                root,
                config => config.Documents.AddSwaggerEndpoint());
        }

        private static void ConfigureInstrumentation(IServiceCollection services)
        {
            services.AddApplicationInsightsInstrumentationTelemetry();
            services.AddLogging();
        }

        private static void ConfigureServiceIdentity(IServiceCollection services, IConfiguration root)
        {
            string legacyAuthConnectionString = root["AzureServicesAuthConnectionString"];
            services.AddServiceIdentityAzureTokenCredentialSourceFromLegacyConnectionString(legacyAuthConnectionString);
            services.AddMicrosoftRestAdapterForServiceIdentityAccessTokenSource();
        }

        private static void ConfigureAuthenticationStrategies(IServiceCollection services)
        {
#if DEBUG
            services.AddClaimsProviderStrategy<UnsafeJwtAuthorizationBearerTokenStrategy>();
            services.AddClaimsProviderStrategy<MarainClaimsStrategy>();
#endif

            services.AddClaimsProviderStrategy<EasyAuthJwtStrategy>();
        }
    }
}