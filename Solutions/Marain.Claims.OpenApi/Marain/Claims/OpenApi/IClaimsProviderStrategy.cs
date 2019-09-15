// <copyright file="IClaimsProviderStrategy.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.OpenApi
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface implemented by types that offer a strategy for the claims provider to build a list of claims.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    public interface IClaimsProviderStrategy<TRequest>
    {
        /// <summary>
        /// Builds a claims identity.
        /// </summary>
        /// <param name="request">The incoming request.</param>
        /// <returns>A populated <see cref="ClaimsIdentity"/>.</returns>
        Task<ClaimsIdentity> BuildClaimsIdentityAsync(TRequest request);
    }
}
