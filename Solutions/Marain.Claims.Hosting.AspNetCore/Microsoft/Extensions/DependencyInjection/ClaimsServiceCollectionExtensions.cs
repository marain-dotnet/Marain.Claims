// <copyright file="ClaimsServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using System.Linq;
    using Corvus.Azure.Storage.Tenancy;
    using Corvus.Identity.ManagedServiceIdentity.ClientAuthentication;
    using Marain.Claims.Hosting.AspNetCore;
    using Marain.Claims.OpenApi;
    using Marain.Tenancy.Client;
    using Menes;
    using Menes.AccessControlPolicies;
    using Microsoft.Extensions.Configuration;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;

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

            // Work around the fact that the tenancy client currently tries to fetch the root tenant on startup.
            services.AddMarainServiceConfiguration();

            services.AddMarainServicesTenancy();
            services.AddSingleton(sp => sp.GetRequiredService<IConfiguration>().GetSection("TenancyClient").Get<TenancyClientOptions>());
            services.AddTenantProviderServiceClient();

            services.AddAzureManagedIdentityBasedTokenSource(
                sp => new AzureManagedIdentityTokenSourceOptions
                {
                    AzureServicesAuthConnectionString = sp.GetRequiredService<IConfiguration>()["AzureServicesAuthConnectionString"],
                });

            services.AddTenantCloudBlobContainerFactory(
                sp => new TenantCloudBlobContainerFactoryOptions
                {
                    AzureServicesAuthConnectionString = sp.GetRequiredService<IConfiguration>()["AzureServicesAuthConnectionString"],
                });

            services.AddTenantedBlobContainerClaimsStore();

            services.AddJsonNetSerializerSettingsProvider();
            services.AddJsonNetPropertyBag();
            services.AddJsonNetCultureInfoConverter();
            services.AddJsonNetDateTimeOffsetToIso8601AndUnixTimeConverter();
            services.AddSingleton<JsonConverter>(new StringEnumConverter(new CamelCaseNamingStrategy()));

            services.AddSingleton<ClaimPermissionsService>();
            services.AddSingleton<IOpenApiService, ClaimPermissionsService>(s => s.GetRequiredService<ClaimPermissionsService>());
            services.AddSingleton<ResourceAccessRuleSetService>();
            services.AddSingleton<IOpenApiService, ResourceAccessRuleSetService>(s => s.GetRequiredService<ResourceAccessRuleSetService>());

            services.AddApplicationInsightsInstrumentationTelemetry();

            services.AddOpenApiHttpRequestHosting<SimpleOpenApiContext>((config) =>
            {
                config.Documents.RegisterOpenApiServiceWithEmbeddedDefinition<ClaimPermissionsService>();
                configureHost?.Invoke(config);
            });

            services.AddSingleton<IResourceAccessEvaluator, LocalResourceAccessEvaluator>();

            services.AddOpenApiClaims();

#if DEBUG
            services.AddClaimsProviderStrategy<UnsafeJwtAuthorizationBearerTokenStrategy>();
            services.AddClaimsProviderStrategy<MarainClaimsStrategy>();
#endif

            services.AddClaimsProviderStrategy<EasyAuthJwtStrategy>();

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

            return services;
        }
    }
}
