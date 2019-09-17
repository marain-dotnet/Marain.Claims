// <copyright file="OpenApiClaimsServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using System.Linq;
    using Marain.Claims.OpenApi;
    using Marain.Claims.OpenApi.Internal;
    using Menes;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Extensions for a <see cref="IServiceCollection"/>.
    /// </summary>
    public static class OpenApiClaimsServiceCollectionExtensions
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

        /// <summary>
        /// Adds the services for open API claims.
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddOpenApiClaims(this IServiceCollection services)
        {
            if (services.Any(s => s.ImplementationType == typeof(ClaimsOpenApiContextBuilderComponent)))
            {
                return services;
            }

            services.AddRequestClaimsProvider<HttpRequest>();
            services.AddSingleton<IOpenApiContextBuilderComponent<HttpRequest>, ClaimsOpenApiContextBuilderComponent>();
            return services;
        }
    }
}
