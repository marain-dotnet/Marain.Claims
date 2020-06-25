// <copyright file="MarainClaimsStrategy.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.OpenApi
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Builds a claims identity using the serialized payload from the 'X-MARAIN-CLAIMS' header on the request.
    /// </summary>
    /// <remarks>
    /// The 'X-MARAIN-CLAIMS' header should be a JSON serialized <see cref="JwtPayload"/>. The 'name' claim is used
    /// as the identity name type, and the 'roles' claim is used as the identity role type.
    /// Example serialized payload:
    /// {"name": "mike", "roles": ["admin", "editor"], "company": "endjin"}.
    /// </remarks>
    public class MarainClaimsStrategy : IClaimsProviderStrategy<HttpRequest>
    {
        private const string HeaderKey = "X-MARAIN-CLAIMS";

        /// <summary>
        /// Builds a claims identity.
        /// </summary>
        /// <param name="request">The incoming <see cref="HttpRequest" />.</param>
        /// <returns>A populated <see cref="ClaimsIdentity" />.</returns>
        public Task<ClaimsIdentity> BuildClaimsIdentityAsync(HttpRequest request)
        {
            ClaimsIdentity result = null;

            if (request.Headers.ContainsKey(HeaderKey))
            {
                var jwtPayload = JwtPayload.Deserialize(request.Headers[HeaderKey]);

                result = new ClaimsIdentity(jwtPayload.Claims, "marainclaims", "name", "roles");
            }

            return Task.FromResult(result);
        }
    }
}
