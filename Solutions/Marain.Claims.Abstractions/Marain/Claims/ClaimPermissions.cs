// <copyright file="ClaimPermissions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Corvus.Extensions;
    using Newtonsoft.Json;

    /// <summary>
    /// Class defining the resource access permissions granted for identities holding a particular claim.
    /// </summary>
    /// <remarks>
    /// <para>
    /// An application or service that wishes to implement access control based on, for example, Azure AD application
    /// role membership would define a <see cref="ClaimPermissions"/> for each role. When a request comes into the
    /// service, it would inspect the access token presented by the client for a <c>roles</c> claim, which contains
    /// a list of role ids. It would look up the <see cref="ClaimPermissions"/> for each of those ids to determine
    /// whether the client is allowed to access the resource it is trying to use in the particular way it is trying to
    /// use it.
    /// </para>
    /// <para>
    /// Rules can be defined in two ways. You can add them to the <see cref="ResourceAccessRules"/> property. You can also
    /// refer to a <see cref="ResourceAccessRuleSet"/>, which is a set of rules with its own identity that may be shared by
    /// multiple <see cref="ClaimPermissions"/>. For example, suppose you define three roles, "Developer",
    /// "Administrator", and "Stakeholder". There might be some overlap across these roles - for example, you might
    /// want all three user types to be able to read all "Work Item" resources, but for only "Developer" and
    /// "Administrator" users to be able to modify them. So you might define two <see cref="ResourceAccessRuleSet"/>s
    /// called "ReadWorkItems" and "WriteWorkItems". The <see cref="ClaimPermissions"/> you define for the "Developer"
    /// and "Administrator" roles would each refer to both "ReadWorkItems" and "WriteWorkItems" in their
    /// <see cref="ResourceAccessRuleSets"/>, while the one for the "Stakeholder" role would contain "ReadWorkItems" but not
    /// "WriteWorkItems".
    /// </para>
    /// </remarks>
    public class ClaimPermissions
    {
        /// <summary>
        /// The registered content type used when this object is serialized/deserialized.
        /// </summary>
        public const string RegisteredContentType = "application/vnd.marain.claims.claimpermissions";

        private IList<ResourceAccessRule> resourceAccessRules;
        private IList<ResourceAccessRuleSet> resourceAccessRuleSets;

        /// <summary>
        /// Gets the content type used when this object is serialized/deserialized.
        /// </summary>
#pragma warning disable CA1822 // Mark members as static - content handling reads this with reflection
        public string ContentType => RegisteredContentType;
#pragma warning restore CA1822

        /// <summary>
        ///  Gets or sets the unique identifier of the claim for which permissions are being defined.
        /// </summary>
        /// <remarks>
        /// <para>
        /// It is up to individual applications to decide what this identifier signifies. For example, it might be
        /// an Azure AD application role id. It might be an Azure AD principal id. Or it could be an email address.
        /// </para>
        /// </remarks>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the ETag for the claim permissions.
        /// </summary>
        public string ETag { get; set; }

        /// <summary>
        /// Gets or sets the access rules that apply directly to this claim permissions.
        /// </summary>
        public IList<ResourceAccessRule> ResourceAccessRules
        {
            get { return this.resourceAccessRules ??= new List<ResourceAccessRule>(); }
            set { this.resourceAccessRules = value; }
        }

        /// <summary>
        /// Gets or sets the resource access rule sets for this claim permissions.
        /// </summary>
        public IList<ResourceAccessRuleSet> ResourceAccessRuleSets
        {
            get { return this.resourceAccessRuleSets ??= new List<ResourceAccessRuleSet>(); }
            set { this.resourceAccessRuleSets = value; }
        }

        /// <summary>
        /// Gets all distinct resource access rules, based on both the claim's direct resource access rules and rules
        /// that are part of the claim's resource access rule sets.
        /// </summary>
        [JsonIgnore]
        public IList<ResourceAccessRule> AllResourceAccessRules
        {
            get
            {
                return this.ResourceAccessRules.Concatenate(this.ResourceAccessRuleSets.SelectMany(x => x.Rules)).Distinct().ToList();
            }
        }

        /// <summary>
        /// Checks whether this <see cref="ClaimPermissions"/> grants permission to use a resource in a particular way.
        /// </summary>
        /// <param name="resourceUri">The URI of the target resource.</param>
        /// <param name="accessType">The type of access to test for.</param>
        /// <returns>True if the claim permissions grants permission.</returns>
        public bool HasPermissionFor(Uri resourceUri, string accessType)
        {
            IEnumerable<ResourceAccessRule> matchingRules = this.AllResourceAccessRules
                .Where(x => x.IsMatch(resourceUri, accessType));

            return matchingRules.AllAndAtLeastOne(x => x.Permission == Permission.Allow);
        }
    }
}
