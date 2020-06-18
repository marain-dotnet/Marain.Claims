// <copyright file="MarainClaimsClientOptions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.Client
{
    using System;

    /// <summary>
    /// Options for configuring <see cref="ClaimsService"/> via <see cref="ClaimsClientServiceCollectionExtensions"/>
    /// </summary>
    public class ClaimsClientOptions
    {
        /// <summary>
        /// Gets or sets the base URI for the Claims service
        /// </summary>
        public Uri BaseUri { get; set; }

        /// <summary>
        /// Gets or sets the resource ID of the AAD app used for the easyAuth for
        /// the claims service.
        /// </summary>
        public string ResourceIdForMsiAuthentication { get; set; }
    }
}
