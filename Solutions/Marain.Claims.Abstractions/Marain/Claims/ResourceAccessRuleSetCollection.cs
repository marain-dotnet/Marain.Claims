// <copyright file="ResourceAccessRuleSetCollection.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims
{
    using System.Collections.Generic;

    /// <summary>
    /// A set of claim permissions.
    /// </summary>
    public class ResourceAccessRuleSetCollection
    {
        private List<ResourceAccessRuleSet> ruleSets;

        /// <summary>
        /// Gets or sets the claim permissions in the set.
        /// </summary>
        public List<ResourceAccessRuleSet> RuleSets
        {
            get => this.ruleSets ??= new List<ResourceAccessRuleSet>();
            set => this.ruleSets = value;
        }
    }
}
