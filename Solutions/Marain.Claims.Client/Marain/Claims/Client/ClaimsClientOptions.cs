// <copyright file="ClaimsClientOptions.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

using System;

namespace Marain.Claims.Client
{
    public class ClaimsClientOptions
    {
        /// <summary>
        /// Gets or sets the base URL of the tenancy service.
        /// </summary>
        public Uri ClaimsServiceBaseUri { get; set; }

        /// <summary>
        /// Gets or sets the resource ID to use when asking the Managed Identity system for a token
        /// with which to communicate with the tenancy service. This is typically the App ID of the
        /// application created for securing access to the tenancy service.
        /// </summary>
        /// <remarks>
        /// If this is null, no attempt will be made to secure communication with the claims
        /// service. This may be appropriate for local development scenarios.
        /// </remarks>
        public string ResourceIdForMsiAuthentication { get; set; }
    }
}