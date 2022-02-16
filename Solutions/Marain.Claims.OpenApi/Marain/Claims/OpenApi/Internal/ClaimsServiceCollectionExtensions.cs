// <copyright file="ClaimsServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.OpenApi.Internal
{
    using System.Linq;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Extensions for a <see cref="IServiceCollection"/>.
    /// </summary>
    public static class ClaimsServiceCollectionExtensions
    {
        /// <summary>
        /// Adds a request claims provider that uses all registered <see cref="IClaimsProviderStrategy{TRequest}"/>
        /// to build a claims principal.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <param name="services">The service collection to add to.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddRequestClaimsProvider<TRequest>(this IServiceCollection services)
        {
            if (services.Any(s => s.ImplementationType == typeof(RequestClaimsProvider<TRequest>)))
            {
                return services;
            }

            services.AddSingleton<IRequestClaimsProvider<TRequest>, RequestClaimsProvider<TRequest>>();
            return services;
        }

        /// <summary>
        /// Adds a claims provider strategy for.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request.</typeparam>
        /// <typeparam name="TStrategy">Type of the <see cref="IClaimsProviderStrategy{TRequest}"/> to add.</typeparam>
        /// <param name="services">The service collection to add to.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddClaimsProviderStrategy<TRequest, TStrategy>(this IServiceCollection services)
            where TStrategy : class, IClaimsProviderStrategy<TRequest>
        {
            if (services.Any(s => s.ImplementationType == typeof(TStrategy)))
            {
                return services;
            }

            services.AddSingleton<IClaimsProviderStrategy<TRequest>, TStrategy>();
            return services;
        }
    }
}