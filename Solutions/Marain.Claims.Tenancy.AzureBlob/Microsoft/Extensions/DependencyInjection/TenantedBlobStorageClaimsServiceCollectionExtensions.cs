// <copyright file="TenantedBlobStorageClaimsServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using System.Linq;
    using Corvus.ContentHandling;
    using Marain.Claims;
    using Marain.Claims.Internal;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// Service collection extensions to add implementations of claims stores.
    /// </summary>
    public static class TenantedBlobStorageClaimsServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the tenancy repository-based implementation of <see cref="IClaimPermissionsStore"/>
        /// and <see cref="IResourceAccessRuleSetStore"/> to the service container.
        /// </summary>
        /// <param name="services">
        /// The collection.
        /// </param>
        /// <returns>
        /// The configured <see cref="IServiceCollection"/>.
        /// </returns>
        public static IServiceCollection AddTenantedClaimsStoreOnAzureBlobStorage(
            this IServiceCollection services)
        {
            if (services.Any(s => s.ServiceType is IPermissionsStoreFactory))
            {
                return services;
            }

            services.AddContent(contentFactory =>
            {
                contentFactory.RegisterTransientContent<ResourceAccessRuleSet>();
                contentFactory.RegisterTransientContent<ClaimPermissions>();
            });

            services.AddBlobContainerV2ToV3Transition();
            services.AddAzureBlobStorageClientSourceFromDynamicConfiguration();

            services.AddJsonNetSerializerSettingsProvider();
            services.AddJsonNetPropertyBag();
            services.AddJsonNetCultureInfoConverter();
            services.AddJsonNetDateTimeOffsetToIso8601AndUnixTimeConverter();
            services.AddSingleton<JsonConverter>(new StringEnumConverter(new CamelCaseNamingStrategy()));

            services.AddSingleton<IPermissionsStoreFactory, BlobContainerPermissionsStoreFactory>();

            return services;
        }
    }
}