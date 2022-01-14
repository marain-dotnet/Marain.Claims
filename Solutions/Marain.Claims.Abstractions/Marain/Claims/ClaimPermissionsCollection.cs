// <copyright file="ClaimPermissionsCollection.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims
{
    using System.Collections.Generic;

    /// <summary>
    /// A set of claim permissions.
    /// </summary>
    public class ClaimPermissionsCollection
    {
        private List<ClaimPermissions> permissions;

        /// <summary>
        /// Gets or sets the claim permissions in the set.
        /// </summary>
        public List<ClaimPermissions> Permissions
        {
            get => this.permissions ??= new List<ClaimPermissions>();
            set => this.permissions = value;
        }
    }
}
