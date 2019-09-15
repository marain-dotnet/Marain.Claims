// <copyright file="UnsafeJwtAuthorizationBearerTokenStrategy.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.OpenApi
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Marain.Claims.OpenApi;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Primitives;

    /// <summary>
    /// Builds a claims identity using the JWT provided as a bearer token in the Authorization header of the request.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This provider is intended only for use in local development scenarios and should never be used in production.
    /// It does not verify the signature at all, it just accepts the token as is. This is useful when developing
    /// Azure functions or applications that are intended to use EasyAuth; when running locally this provider can be
    /// used in place of the EasyAuth strategies to simulate the fact that EasyAuth validates your tokens for you
    /// prior to your own code being invoked.
    /// </para>
    /// </remarks>
    public class UnsafeJwtAuthorizationBearerTokenStrategy : IClaimsProviderStrategy<HttpRequest>
    {
        private readonly ILogger<UnsafeJwtAuthorizationBearerTokenStrategy> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsafeJwtAuthorizationBearerTokenStrategy"/> class.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        public UnsafeJwtAuthorizationBearerTokenStrategy(ILogger<UnsafeJwtAuthorizationBearerTokenStrategy> logger)
        {
            this.logger = logger;
        }

        /// <inheritdoc/>
        public Task<ClaimsIdentity> BuildClaimsIdentityAsync(HttpRequest request)
        {
            ClaimsIdentity result = null;

            if (request.Headers.TryGetValue("Authorization", out StringValues header) && header.Count > 0)
            {
                string val = header[0];

                if (val.StartsWith("Bearer "))
                {
                    var jwt = new JwtSecurityToken(val.Substring(7));

                    result = new ClaimsIdentity(jwt.Claims, "azuread", "name", "roles");
                }
                else
                {
                    this.logger.LogInformation("Unable to authenticate request: Authorization header does not begin with 'Bearer: '");
                }
            }
            else
            {
                this.logger.LogInformation("Unable to authenticate request: no Authorization header found");
            }

            return Task.FromResult(result);
        }
    }
}
