// <copyright file="ClaimsClientServiceCollectionExtensions.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

namespace Marain.Claims.Client
{
    using System;
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
        /// <param name="baseUri">The base URI of the Operations control service.</param>
        /// <param name="resourceIdForMsiAuthentication">
        /// The resource id to use when obtaining an authentication token representing the
        /// hosting service's identity. Pass null to run without authentication.
        /// </param>
        /// <returns>The modified service collection.</returns>
        public static IServiceCollection AddClaimsClient(
            this IServiceCollection services,
            Uri baseUri,
            string resourceIdForMsiAuthentication = null)
        {
            return resourceIdForMsiAuthentication == null
                ? services.AddSingleton<IClaimsService>(new UnauthenticatedClaimsService(baseUri))
                : services.AddSingleton<IClaimsService>(sp =>
                    new ClaimsService(baseUri, new TokenCredentials(
                        new ServiceIdentityTokenProvider(
                            sp.GetRequiredService<IServiceIdentityTokenSource>(),
                            resourceIdForMsiAuthentication))));
        }
    }
}
