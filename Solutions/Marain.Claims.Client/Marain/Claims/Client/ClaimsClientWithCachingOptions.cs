// <copyright file="ClaimsClientWithCachingOptions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.Client
{
    using System;
    using System.Collections.Generic;
    using System.Net;

    /// <summary>
    /// Options for configuring <see cref="ClaimsService"/> via <see cref="ClaimsClientServiceCollectionExtensions.AddClaimsClientWithCaching(Microsoft.Extensions.DependencyInjection.IServiceCollection, System.Func{System.IServiceProvider, ClaimsClientWithCachingOptions})"/>.
    /// </summary>
    public class ClaimsClientWithCachingOptions : ClaimsClientOptions
    {
        /// <summary>
        /// Gets or sets the dictionary containing mappings of HTTP status codes to cache expiration periods.
        /// </summary>
        public Dictionary<HttpStatusCode, TimeSpan> CacheExpirationPerHttpResponseCode { get; set; }
    }
}