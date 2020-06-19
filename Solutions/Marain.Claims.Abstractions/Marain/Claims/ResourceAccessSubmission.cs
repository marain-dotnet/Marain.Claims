// <copyright file="ResourceAccessSubmission.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims
{
    /// <summary>
    /// This describes an aspiration to access a particular resource, in a particular way.
    /// </summary>
    public class ResourceAccessSubmission
    {
        /// <summary>
        /// Gets or sets the claim permissions ID.
        /// </summary>
        public string ClaimPermissionsId { get; set; }

        /// <summary>
        /// Gets or sets the uri for the resource for which to determine permissions.
        /// </summary>
        public string ResourceUri { get; set; }

        /// <summary>
        /// Gets or sets the type of access required to the resource.
        /// </summary>
        public string ResourceAccessType { get; set; }
    }
}
