// <copyright file="IRequestClaimsProvider.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>
namespace Marain.Claims.OpenApi
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface implemented by providers that build a claims principal for the incoming request.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    public interface IRequestClaimsProvider<TRequest>
    {
        /// <summary>
        /// Builds a claims principal using registered claims provider strategies.
        /// </summary>
        /// <param name="request">The incoming request.</param>
        /// <returns>
        /// The populated <see cref="ClaimsPrincipal"/>.
        /// </returns>
        Task<ClaimsPrincipal> BuildClaimsPrincipalAsync(TRequest request);
    }
}