// <copyright file="ClaimsSerializationServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using Corvus.ContentHandling;
    using Marain.Claims.OpenApi;

    /// <summary>
    /// Extension method for adding claims objects to the service
    /// collection so they can be serialized and deserialized by their
    /// content type.
    /// </summary>
    public static class ClaimsSerializationServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the standard set of core claims types.
        /// </summary>
        /// <param name="services">
        /// The <see cref="IServiceCollection" /> to add the types to.
        /// </param>
        /// <returns>
        /// The <see cref="IServiceCollection" />.
        /// </returns>
        public static IServiceCollection RegisterCoreClaimsContentTypes(this IServiceCollection services)
        {
            services.AddContent(factory => factory.RegisterTransientContent<ClaimPermissionsBatchRequestItem>());

            return services;
        }
    }
}