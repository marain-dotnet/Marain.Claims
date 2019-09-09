// <copyright file="RequestClaimsProvider.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.Internal
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    /// <summary>
    /// Provider for building a claims principal for the incoming request.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    public class RequestClaimsProvider<TRequest> : IRequestClaimsProvider<TRequest>
    {
        private readonly IEnumerable<IClaimsProviderStrategy<TRequest>> claimsProviderStrategies;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestClaimsProvider{TRequest}"/> class.
        /// </summary>
        /// <param name="claimsProviderStrategies">The collection of registered <see cref="IClaimsProviderStrategy{TRequest}"/> instances.</param>
        public RequestClaimsProvider(IEnumerable<IClaimsProviderStrategy<TRequest>> claimsProviderStrategies)
        {
            this.claimsProviderStrategies = claimsProviderStrategies;
        }

        /// <summary>
        /// Builds a claims principal using registered claims provider strategies.
        /// </summary>
        /// <param name="request">The incoming request.</param>
        /// <returns>
        /// The populated <see cref="ClaimsPrincipal" />.
        /// </returns>
        public async Task<ClaimsPrincipal> BuildClaimsPrincipalAsync(TRequest request)
        {
            IEnumerable<Task<ClaimsIdentity>> identityTasks = this.claimsProviderStrategies
                .Select(x => x.BuildClaimsIdentityAsync(request))
                .ToList();  // Avoid double evaluation by Task.WhenAll and then ClaimsPrincipal
            await Task.WhenAll(identityTasks).ConfigureAwait(false);

            IEnumerable<ClaimsIdentity> identities = identityTasks.Where(x => x.Result != null).Select(x => x.Result);

            return new ClaimsPrincipal(identities);
        }
    }
}
