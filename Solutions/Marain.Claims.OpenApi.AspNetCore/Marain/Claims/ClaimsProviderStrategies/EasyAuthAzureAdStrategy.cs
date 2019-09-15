// <copyright file="EasyAuthAzureAdStrategy.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Builds a claims identity using the 'X-MS-TOKEN-AAD-ID-TOKEN' header on the request.
    /// </summary>
    /// <remarks>
    /// The 'X-MS-TOKEN-AAD-ID-TOKEN' header value is a JWT. It is set by Easy Auth when enabled
    /// with the Azure AD provider on Azure App Service. The 'name' claim is used
    /// as the identity name type, and the 'roles' claim is used as the identity role type.
    /// </remarks>
    public class EasyAuthAzureAdStrategy : IClaimsProviderStrategy<HttpRequest>
    {
        /// <summary>
        /// Builds a claims identity.
        /// </summary>
        /// <param name="request">The incoming <see cref="HttpRequest" />.</param>
        /// <returns>A populated <see cref="ClaimsIdentity" />.</returns>
        public Task<ClaimsIdentity> BuildClaimsIdentityAsync(HttpRequest request)
        {
            ClaimsIdentity result = null;

            if (request.Headers.ContainsKey("X-MS-TOKEN-AAD-ID-TOKEN"))
            {
                var jwt = new JwtSecurityToken(request.Headers["X-MS-TOKEN-AAD-ID-TOKEN"]);

                result = new ClaimsIdentity(jwt.Claims, "azuread", "name", "roles");
            }

            return Task.FromResult(result);
        }
    }
}
