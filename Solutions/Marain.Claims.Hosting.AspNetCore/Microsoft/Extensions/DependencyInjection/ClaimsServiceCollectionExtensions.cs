// <copyright file="ClaimsServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using System.Linq;
    using Corvus.ContentHandling;
    using Endjin.Claims.Hosting;
    using Marain.Claims;
    using Marain.Claims.Client;
    using Marain.Claims.OpenApi;
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
        public static IServiceCollection AddTenantedClaimsApi(
            this IServiceCollection services,
            IConfiguration rootTenantDefaultConfiguration,
            Action<IOpenApiHostConfiguration> configureHost = null)
        {
            if (services.Any(s => typeof(ClaimPermissionsService).IsAssignableFrom(s.ImplementationType)))
            {
                return services;
            }

            services.AddLogging();
            services.AddTenantProviderBlobStore();
            services.AddTenantedBlobContainerClaimsStore(rootTenantDefaultConfiguration);
            services.AddSingleton<ClaimPermissionsService>();
            services.AddSingleton<IOpenApiService, ClaimPermissionsService>(s => s.GetRequiredService<ClaimPermissionsService>());
            services.AddSingleton<ResourceAccessRuleSetService>();
            services.AddSingleton<IOpenApiService, ResourceAccessRuleSetService>(s => s.GetRequiredService<ResourceAccessRuleSetService>());
            services.AddOpenApiHttpRequestHosting<SimpleOpenApiContext>((config) =>
            {
                config.Documents.RegisterOpenApiServiceWithEmbeddedDefinition<ClaimPermissionsService>();
                configureHost?.Invoke(config);
            });

            services.AddSingleton<IClaimPermissionsEvaluator, PermissionsEvaluatorBridge>();
            services.AddSingleton<IClaimsService, LocalClaimsService>();

            services.AddOpenApiClaims();

#if DEBUG
            services.AddClaimsProviderStrategy<UnsafeJwtAuthorizationBearerTokenStrategy>();
#endif

            services.AddClaimsProviderStrategy<EasyAuthJwtStrategy>();

            string[] openOperationIds =
            {
                    ClaimPermissionsService.GetClaimPermissionsPermissionOperationId,
                    ClaimPermissionsService.GetClaimPermissionsPermissionBatchOperationId,
                    ClaimPermissionsService.InitializeTenantOperationId,
                    Menes.Internal.SwaggerService.SwaggerOperationId,
            };
            services.AddRoleBasedOpenApiAccessControlWithPreemptiveExemptions(
                new ExemptOperationIdsAccessPolicy(openOperationIds),
                ClaimPermissionsService.ClaimsResourceTemplate);

            return services;
        }
    }
}
