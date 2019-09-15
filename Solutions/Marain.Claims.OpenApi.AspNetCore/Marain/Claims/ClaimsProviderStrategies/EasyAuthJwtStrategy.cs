// <copyright file="EasyAuthJwtStrategy.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.ClaimsProviderStrategies
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Builds a claims identity using the 'X-MS-CLIENT-PRINCIPAL' header on the request.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The 'X-MS-CLIENT-PRINCIPAL' header value is the payload of a JWT. It is set by Easy Auth
    /// when enabled with the Azure AD provider on Azure App Service. The 'name' claim is used
    /// as the identity name type, and the 'roles' claim is used as the identity role type.
    /// </para>
    /// <para>
    /// Note that this is not documented. Unfortunately, the approach that <em>is</em> documented
    /// and which is used by <see cref="EasyAuthAzureAdStrategy"/> turns out not to work for
    /// service-to-service authentication because the <c>X-MS-TOKEN-AAD-ID-TOKEN</c> header that
    /// relies on isn't present in those circumstances. (A pity, because that header is actually
    /// documented.)
    /// </para>
    /// </remarks>
    public class EasyAuthJwtStrategy : IClaimsProviderStrategy<HttpRequest>
    {
        /// <summary>
        /// Builds a claims identity.
        /// </summary>
        /// <param name="request">The incoming <see cref="HttpRequest" />.</param>
        /// <returns>A populated <see cref="ClaimsIdentity" />.</returns>
        public Task<ClaimsIdentity> BuildClaimsIdentityAsync(HttpRequest request)
        {
            ClaimsIdentity result = null;

            if (request.Headers.ContainsKey("X-MS-CLIENT-PRINCIPAL"))
            {
                var payload = JwtPayload.Base64UrlDeserialize(request.Headers["X-MS-CLIENT-PRINCIPAL"]);
                var claims = payload.Claims.Where(x => x.Type == "claims").Select(x => ConvertToClaim(x.Value)).ToList();

                result = new ClaimsIdentity(claims, "azuread", "name", "roles");
            }

            return Task.FromResult(result);
        }

        private static Claim ConvertToClaim(string input)
        {
            var claim = JObject.Parse(input);
            return new Claim(claim["typ"].ToString(), claim["val"].ToString());
        }
    }
}
