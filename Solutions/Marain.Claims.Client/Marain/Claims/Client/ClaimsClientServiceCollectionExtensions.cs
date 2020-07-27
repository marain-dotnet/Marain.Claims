// <copyright file="ClaimsClientServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.Client
{
    using System;
    using System.Net.Http;
    using Cimpress.Extensions.Http.Caching.InMemory;
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
        /// <param name="getOptions">A callback method to retrieve options for the client.</param>
        /// <returns>The modified service collection.</returns>
        public static IServiceCollection AddClaimsClient(
            this IServiceCollection services,
            Func<IServiceProvider, ClaimsClientOptions> getOptions)
        {
            return services.AddSingleton<IClaimsService>(sp =>
            {
                ClaimsClientOptions options = getOptions(sp);
                IServiceIdentityTokenSource serviceIdentityTokenSource = sp.GetRequiredService<IServiceIdentityTokenSource>();
                return options.ResourceIdForMsiAuthentication == null
                   ? new UnauthenticatedClaimsService(options.BaseUri)
                   : new ClaimsService(options.BaseUri, new TokenCredentials(
                           new ServiceIdentityTokenProvider(
                               serviceIdentityTokenSource,
                               options.ResourceIdForMsiAuthentication)));
            });
        }

        /// <summary>
        /// Adds the Claims client (with caching enabled) to a service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="getOptions">A callback method to retrieve options for the client.</param>
        /// <returns>The modified service collection.</returns>
        public static IServiceCollection AddClaimsClientWithCaching(
           this IServiceCollection services,
           Func<IServiceProvider, ClaimsClientWithCachingOptions> getOptions)
        {
            return services.AddSingleton<IClaimsService>(sp =>
            {
                ClaimsClientWithCachingOptions options = getOptions(sp);

                var handler = new InMemoryCacheHandler(new HttpClientHandler(), options.CacheExpirationPerHttpResponseCode);
                IServiceIdentityTokenSource serviceIdentityTokenSource = sp.GetRequiredService<IServiceIdentityTokenSource>();
                return options.ResourceIdForMsiAuthentication == null
                ? new UnauthenticatedClaimsService(options.BaseUri, handler)
                : new ClaimsService(
                    options.BaseUri,
                    new TokenCredentials(
                        new ServiceIdentityTokenProvider(
                            serviceIdentityTokenSource,
                            options.ResourceIdForMsiAuthentication)),
                    handler);
            });
        }
    }
}
