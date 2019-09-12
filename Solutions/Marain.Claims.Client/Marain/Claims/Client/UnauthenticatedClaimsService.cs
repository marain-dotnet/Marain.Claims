// <copyright file="UnauthenticatedClaimsService.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

namespace Marain.Claims.Client
{
    using System;
    using System.Net.Http;

    /// <summary>
    /// Claims API client for use in scenarios where authentication is not required.
    /// </summary>
    /// <remarks>
    /// <para>
    /// In scenarios in which inter-service communication is secured at a networking level, it
    /// might be unnecessary to authenticate requests. The base proxy type supports this but only
    /// through protected constructors. This type makes a suitable constructor available publicly.
    /// </para>
    /// </remarks>
    public class UnauthenticatedClaimsService : ClaimsService
    {
        /// <summary>
        /// Create an <see cref="UnauthenticatedClaimsService"/>.
        /// </summary>
        /// <param name="baseUri">The base URI of the Opeartions control service.</param>
        /// <param name="handlers">Optional request processing handlers.</param>
        public UnauthenticatedClaimsService(Uri baseUri, params DelegatingHandler[] handlers)
            : base(baseUri, handlers)
        {
        }
    }
}
