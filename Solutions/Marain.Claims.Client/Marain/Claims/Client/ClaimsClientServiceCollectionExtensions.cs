// <copyright file="ClaimsClientServiceCollectionExtensions.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

namespace Marain.Claims.Client
{
    using System;
    using System.Linq;
    using Corvus.Identity.ManagedServiceIdentity.ClientAuthentication;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Rest;

    /// <summary>
    /// DI initialization for clients of the Claims service.
    /// </summary>
    public static class ClaimsClientServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the Claims client to a service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The modified service collection.</returns>
        /// <remarks>
        /// This requires the <see cref="ClaimsClientOptions"/> to be available from DI in order
        /// to discover the base URI of the Claims service, and, if required, to
        /// specify the resource id to use when obtaining an authentication token representing the
        /// hosting service's identity.
        /// </remarks>
        public static IServiceCollection AddClaimsClient(
            this IServiceCollection services)
        {
            if (services.Any(s => s.ServiceType == typeof(IClaimsService)))
            {
                return services;
            }

            services.AddSingleton((Func<IServiceProvider, IClaimsService>)(sp =>
            {
                ClaimsClientOptions options = sp.GetRequiredService<ClaimsClientOptions>();

                ClaimsService service;
                if (string.IsNullOrWhiteSpace(options.ResourceIdForMsiAuthentication))
                {
                    service = new UnauthenticatedClaimsService(options.ClaimsServiceBaseUri);
                }
                else
                {
                    service = new ClaimsService(options.ClaimsServiceBaseUri, new TokenCredentials(
                        new ServiceIdentityTokenProvider(
                            sp.GetRequiredService<IServiceIdentityTokenSource>(),
                            options.ResourceIdForMsiAuthentication)));
                }

                return service;
            }));

            return services;
        }
    }
}
