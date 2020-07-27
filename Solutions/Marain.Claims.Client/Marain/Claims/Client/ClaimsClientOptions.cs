// <copyright file="ClaimsClientOptions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.Client
{
    using System;

    /// <summary>
    /// Options for configuring <see cref="ClaimsService"/> via <see cref="ClaimsClientServiceCollectionExtensions.AddClaimsClient(Microsoft.Extensions.DependencyInjection.IServiceCollection, System.Func{System.IServiceProvider, ClaimsClientOptions})"/>.
    /// </summary>
    public class ClaimsClientOptions
    {
        /// <summary>
        /// Gets or sets the base URI for the Claims service.
        /// </summary>
        public Uri BaseUri { get; set; }

        /// <summary>
        /// Gets or sets the resource ID of the AAD app used for the easyAuth for
        /// the claims service.
        /// </summary>
        public string ResourceIdForMsiAuthentication { get; set; }
    }
}
