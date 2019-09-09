// <copyright file="ResourceAccessRuleSet.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims
{
    using System.Collections.Generic;

    /// <summary>
    /// Class representing a set of resource access rules.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This makes it possible to define a common set of rules that is shared by multiple
    /// <see cref="ClaimPermissions"/>.
    /// </para>
    /// </remarks>
    public class ResourceAccessRuleSet
    {
        /// <summary>
        /// The registered content type used when this object is serialized/deserialized.
        /// </summary>
        public const string RegisteredContentType = "application/vnd.endjin.claims.resourceaccessruleset";

        private IList<ResourceAccessRule> rules;

        /// <summary>
        /// Gets the content type used when this object is serialized/deserialized.
        /// </summary>
        public string ContentType => RegisteredContentType;

        /// <summary>
        ///  Gets or sets the unique Id of this resource access rule set.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the ETag of this resource access rule set.
        /// </summary>
        public string ETag { get; set; }

        /// <summary>
        ///  Gets or sets the display name of this resource access rule set.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the list of rules contained in this resource access rule set.
        /// </summary>
        public IList<ResourceAccessRule> Rules
        {
            get { return this.rules ?? (this.rules = new List<ResourceAccessRule>()); }
            set { this.rules = value; }
        }
    }
}
