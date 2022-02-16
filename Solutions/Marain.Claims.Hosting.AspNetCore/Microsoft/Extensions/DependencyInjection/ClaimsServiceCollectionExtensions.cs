// <copyright file="ClaimsServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using System.Linq;
    using Marain.Claims.Hosting.AspNetCore;
    using Marain.Claims.OpenApi;
    using Marain.Tenancy.Client;
    using Menes;
    using Menes.AccessControlPolicies;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Extension methods for configuring DI for the Operations Open API services.
    /// </summary>
    public static class ClaimsServiceCollectionExtensions
    {
        /// <summary>
        /// Add services required by the claims API.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="rootTenantDefaultConfiguration">
        /// Configuration section to read root tenant default repository settings from.
        /// </param>
        /// <param name="configureHost">Optional callback for additional host configuration.</param>
        /// <returns>The service collection, to enable chaining.</returns>
        [Obsolete("Use AddTenantedClaimsApiWithOpenApiActionResultHosting, or consider changing to AddTenantedClaimsApiWithAspNetPipelineHosting")]
        public static IServiceCollection AddTenantedClaimsApi(
            this IServiceCollection services,
            IConfiguration rootTenantDefaultConfiguration,
            Action<IOpenApiHostConfiguration> configureHost = null)
        {
            return services.AddTenantedClaimsApiWithOpenApiActionResultHosting(rootTenantDefaultConfiguration, configureHost);
        }

        /// <summary>
        /// Add services required by the claims API.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="rootConfiguration">Application configuration.</param>
        /// <param name="configureHost">Optional callback for additional host configuration.</param>
        /// <returns>The service collection, to enable chaining.</returns>
        public static IServiceCollection AddTenantedClaimsApiWithAspNetPipelineHosting(
            this IServiceCollection services,
            IConfiguration rootConfiguration,
            Action<IOpenApiHostConfiguration> configureHost = null)
        {
            if (services.Any(s => typeof(TenancyService).IsAssignableFrom(s.ServiceType)))
            {
                return services;
            }

            services.AddEverythingExceptHosting(rootConfiguration);

            services.AddOpenApiAspNetPipelineHosting<SimpleOpenApiContext>((config) =>
            {
                config.Documents.RegisterOpenApiServiceWithEmbeddedDefinition<ClaimPermissionsService>();
                configureHost?.Invoke(config);
            });

            return services;
        }

        /// <summary>
        /// Add services required by the claims API.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="rootConfiguration">Application configuration.</param>
        /// <param name="configureHost">Optional callback for additional host configuration.</param>
        /// <returns>The service collection, to enable chaining.</returns>
        public static IServiceCollection AddTenantedClaimsApiWithOpenApiActionResultHosting(
            this IServiceCollection services,
            IConfiguration rootConfiguration,
            Action<IOpenApiHostConfiguration> configureHost = null)
        {
            if (services.Any(s => typeof(TenancyService).IsAssignableFrom(s.ServiceType)))
            {
                return services;
            }

            services.AddEverythingExceptHosting(rootConfiguration);

            services.AddOpenApiActionResultHosting<SimpleOpenApiContext>((config) =>
            {
                config.Documents.RegisterOpenApiServiceWithEmbeddedDefinition<ClaimPermissionsService>();
                configureHost?.Invoke(config);
            });

            return services;
        }

        private static void AddEverythingExceptHosting(
            this IServiceCollection services,
            IConfiguration rootConfiguration)
        {
            services.AddLogging();

            services.AddMarainServiceConfiguration();
            services.AddMarainServicesTenancy();
            services.AddSingleton(rootConfiguration.GetSection("TenancyClient").Get<TenancyClientOptions>());
            services.AddTenantProviderServiceClient(enableResponseCaching: true);

            services.AddSingleton<ClaimPermissionsService>();
            services.AddSingleton<IOpenApiService, ClaimPermissionsService>(s => s.GetRequiredService<ClaimPermissionsService>());
            services.AddSingleton<ResourceAccessRuleSetService>();
            services.AddSingleton<IOpenApiService, ResourceAccessRuleSetService>(s => s.GetRequiredService<ResourceAccessRuleSetService>());

            // Applications are likely to call AddApplicationInsightsInstrumentationTelemetry, but
            // in case they haven't we call the basic instrumentation registration as a fallback.
            services.AddInstrumentation();

            services.AddSingleton<IResourceAccessEvaluator, LocalResourceAccessEvaluator>();
            services.AddClaimsOpenApiContextBuilder();
            string[] openOperationIds =
            {
                    ClaimPermissionsService.GetClaimPermissionsPermissionOperationId,
                    ClaimPermissionsService.GetClaimPermissionsPermissionBatchOperationId,
                    ClaimPermissionsService.InitializeTenantOperationId,
                    Menes.Internal.SwaggerService.SwaggerOperationId,
            };
            services.AddIdentityBasedOpenApiAccessControlWithPreemptiveExemptions(
                new ExemptOperationIdsAccessPolicy(openOperationIds));

            services.RegisterCoreClaimsContentTypes();
        }
    }
}