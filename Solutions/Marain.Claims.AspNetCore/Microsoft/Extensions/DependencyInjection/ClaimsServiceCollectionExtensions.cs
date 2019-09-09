// <copyright file="ClaimsServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using System.Linq;
    using Corvus.ContentHandling;
    using Marain.Claims;
    using Marain.Claims.Internal;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Extensions for a <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ClaimsServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the request claims provider.
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddRequestClaimsProvider(this IServiceCollection services)
        {
            if (services.Any(s => s.ServiceType == typeof(IRequestClaimsProvider<HttpRequest>)))
            {
                return services;
            }

            services.AddContentSerialization(contentFactory =>
            {
                contentFactory.RegisterTransientContent<ResourceAccessRuleSet>();
                contentFactory.RegisterTransientContent<ClaimPermissions>();
            });

            services.AddSingleton<IRequestClaimsProvider<HttpRequest>, RequestClaimsProvider<HttpRequest>>();
            return services;
        }

        /// <summary>
        /// Adds a strategy for the claims provider.
        /// </summary>
        /// <typeparam name="TStrategy">Type of the <see cref="IClaimsProviderStrategy{TRequest}"/> to add.</typeparam>
        /// <param name="services">The service collection to add to.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddClaimsProviderStrategy<TStrategy>(this IServiceCollection services)
            where TStrategy : class, IClaimsProviderStrategy<HttpRequest>
        {
            if (services.Any(s => s.ImplementationType == typeof(TStrategy)))
            {
                return services;
            }

            services.AddSingleton<IClaimsProviderStrategy<HttpRequest>, TStrategy>();
            return services;
        }
    }
}
