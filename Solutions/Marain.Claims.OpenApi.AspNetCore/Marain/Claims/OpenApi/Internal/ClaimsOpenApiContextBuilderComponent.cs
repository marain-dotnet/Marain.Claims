// <copyright file="ClaimsOpenApiContextBuilderComponent.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.OpenApi.Internal
{
    using System.Threading.Tasks;
    using Menes;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Class responsible for setting the claims principal on an <see cref="IOpenApiContext" /> for an incoming request.
    /// </summary>
    public class ClaimsOpenApiContextBuilderComponent : IOpenApiContextBuilderComponent<HttpRequest>
    {
        private readonly IRequestClaimsProvider<HttpRequest> requestClaimsProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimsOpenApiContextBuilderComponent"/> class.
        /// </summary>
        /// <param name="requestClaimsProvider">The <see cref="IRequestClaimsProvider{TRequest}"/> used to build the claims principal.</param>
        public ClaimsOpenApiContextBuilderComponent(IRequestClaimsProvider<HttpRequest> requestClaimsProvider)
        {
            this.requestClaimsProvider = requestClaimsProvider;
        }

        /// <inheritdoc/>
        public async Task BuildAsync(IOpenApiContext context, HttpRequest request, dynamic parameters)
        {
            context.CurrentPrincipal = await this.requestClaimsProvider.BuildClaimsPrincipalAsync(request).ConfigureAwait(false);
        }
    }
}