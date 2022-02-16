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
        /// Adds the request claims provider for HTTP requests.
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddRequestClaimsProvider(this IServiceCollection services)
        {
            return services.AddRequestClaimsProvider<HttpRequest>();
        }

        /// <summary>
        /// Adds a claims provider strategy for HTTP requests.
        /// </summary>
        /// <typeparam name="TStrategy">Type of the <see cref="IClaimsProviderStrategy{TRequest}"/> to add.</typeparam>
        /// <param name="services">The service collection to add to.</param>
        /// <returns>The service collection.</returns>
        public static IServiceCollection AddClaimsProviderStrategy<TStrategy>(this IServiceCollection services)
            where TStrategy : class, IClaimsProviderStrategy<HttpRequest>
        {
            return services.AddClaimsProviderStrategy<HttpRequest, TStrategy>();
        }

        /// <summary>
        /// Adds the services that ensure the Open API context is populated with claims from all
        /// registered <see cref="HttpRequest"/>-based claims provider strategies.
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        /// <returns>The service collection.</returns>
        /// <remarks>
        /// <para>
        /// Hosts that wish to protect an Open API service with claims call this to ensure that
        /// the <see cref="IOpenApiContext.CurrentPrincipal"/> is populated. They will also
        /// call <see cref="AddClaimsProviderStrategy{TStrategy}(IServiceCollection)"/> one or
        /// more times to determine which mechanisms are used to populate it.
        /// </para>
        /// </remarks>
        public static IServiceCollection AddClaimsOpenApiContextBuilder(this IServiceCollection services)
        {
            if (services.Any(s => s.ImplementationType == typeof(ClaimsOpenApiContextBuilderComponent)))
            {
                return services;
            }

            services.AddRequestClaimsProvider();
            services.AddSingleton<IOpenApiContextBuilderComponent<HttpRequest>, ClaimsOpenApiContextBuilderComponent>();
            return services;
        }
    }
}