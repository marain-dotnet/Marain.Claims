// <copyright file="TenantedBlobStorageClaimsServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using System.Linq;
    using Marain.Claims;
    using Marain.Claims.Internal;
    using Microsoft.Extensions.Configuration;

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
        /// <param name="configuration">The configuration from which to initialize the container factory.</param>
        /// <returns>
        /// The configured <see cref="IServiceCollection"/>.
        /// </returns>
        public static IServiceCollection AddTenantedBlobContainerClaimsStore(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            if (configuration is null)
            {
                throw new System.ArgumentNullException(nameof(configuration));
            }

            if (services.Any(s => s.ServiceType is IPermissionsStoreFactory))
            {
                return services;
            }

            services.AddTenantCloudBlobContainerFactory(configuration);
            services.AddJsonSerializerSettings();
            services.AddSingleton<IPermissionsStoreFactory, BlobContainerPermissionsStoreFactory>();

            return services;
        }
    }
}