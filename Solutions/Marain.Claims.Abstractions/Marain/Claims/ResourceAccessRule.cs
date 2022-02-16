// <copyright file="ResourceAccessRule.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims
{
    using System;
    using Microsoft.Extensions.FileSystemGlobbing;

    /// <summary>
    /// Struct representing a rule for resource access permissions. The rule is defined by a resource, an access type,
    /// and a permission. You can then assign rules either directly to a <see cref="ClaimPermissions"/>, or to a
    /// <see cref="ResourceAccessRuleSet"/> (which can in turn be assigned to one or more <see cref="ClaimPermissions"/>).
    /// </summary>
    /// <example>
    /// <para>
    /// To define a rule allowing access of type 'read' to all your 'book' resources, use the following:
    /// </para>
    /// <code>
    /// var resourceUri = new Uri("books/**", UriKind.Relative);
    /// var resource = new Resource(resourceUri, "All Books");
    /// var rule = new ResourceAccessRule("read", resource, Permission.Allow);
    /// </code>
    /// <para>
    /// To define a rule denying access of type 'edit' to your 'Home page' resource, use the following:
    /// </para>
    /// <code>
    /// var resourceUri = new Uri("page/home", UriKind.Relative);
    /// var resource = new Resource(resourceUri, "Home Page");
    /// var rule = new ResourceAccessRule("edit", resource, Permission.Deny);
    /// </code>
    /// </example>
    public struct ResourceAccessRule : IEquatable<ResourceAccessRule>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceAccessRule"/> struct.
        /// </summary>
        /// <param name="accessType">The type of resource usage to which this rule applies.</param>
        /// <param name="resource">The resource the rule applies to.</param>
        /// <param name="permission">Detemines whether permission should be allowed or denied to the resource.</param>
        public ResourceAccessRule(string accessType, Resource resource, Permission permission)
            : this()
        {
            this.AccessType = accessType;
            this.Resource = resource;
            this.Permission = permission;
        }

        /// <summary>
        /// Gets the type of resource access this rule applies to (e.g., <c>read</c>, or <c>delete</c>).
        /// </summary>
        /// <remarks>
        /// <para>
        /// Supports globbing: use '/' to separate segments, use '*' for wildcard in segment,
        /// use '**' for abitrary segment depth, use '..' for parent segment.
        /// </para>
        /// </remarks>
        public string AccessType { get; }

        /// <summary>
        /// Gets the resource this rule applies to.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Supports globbing: use '/' to separate segments, use '*' for wildcard in segment,
        /// use '**' for abitrary segment depth, use '..' for parent segment.
        /// </para>
        /// </remarks>
        public Resource Resource { get; }

        /// <summary>
        /// Gets a value indicating whether this rule allows or denies permission.
        /// </summary>
        public Permission Permission { get; }

        /// <summary>
        /// Compares two <see cref="ResourceAccessRule"/> values.
        /// </summary>
        /// <param name="x">Left <see cref="ResourceAccessRule"/> to compare.</param>
        /// <param name="y">Right <see cref="ResourceAccessRule"/> to compare.</param>
        /// <returns>True if left and right <see cref="ResourceAccessRule"/> are equal.</returns>
        public static bool operator ==(ResourceAccessRule x, ResourceAccessRule y)
        {
            return x.Equals(y);
        }

        /// <summary>
        /// Compares two <see cref="ResourceAccessRule"/> values.
        /// </summary>
        /// <param name="x">Left <see cref="ResourceAccessRule"/> to compare.</param>
        /// <param name="y">Right <see cref="ResourceAccessRule"/> to compare.</param>
        /// <returns>True if left and right <see cref="ResourceAccessRule"/> are not equal.</returns>
        public static bool operator !=(ResourceAccessRule x, ResourceAccessRule y)
        {
            return !x.Equals(y);
        }

        /// <summary>
        /// Get a hash code for the object, based on the access type, resource, and permission.
        /// </summary>
        /// <returns>A hash code.</returns>
        public override int GetHashCode()
        {
            return (this.AccessType.ToLowerInvariant(), this.Resource, this.Permission).GetHashCode();
        }

        /// <summary>
        /// Compares this <see cref="ResourceAccessRule"/> with another.
        /// </summary>
        /// <param name="obj">The other object to compare.</param>
        /// <returns>True if the other object is an <see cref="ResourceAccessRule"/>, and is equal to this.</returns>
        public override bool Equals(object obj)
        {
            return obj is ResourceAccessRule res && this.Equals(res);
        }

        /// <summary>
        /// Compares this <see cref="ResourceAccessRule"/> with another.
        /// </summary>
        /// <param name="other">The other <see cref="ResourceAccessRule"/> to compare.</param>
        /// <returns>True if left and right <see cref="ResourceAccessRule"/> are equal.</returns>
        public bool Equals(ResourceAccessRule other)
        {
            return
                (string.Compare(this.AccessType, other.AccessType, true) == 0)
                && (this.Resource == other.Resource)
                && (this.Permission == other.Permission);
        }

        /// <summary>
        /// Determines whether this permission rule is a match for the target resource name and claim type.
        /// Uses globbing to match to pattern.
        /// </summary>
        /// <param name="resourceUri">The URI of the target resource.</param>
        /// <param name="accessType">The claim type of the target resource.</param>
        /// <returns>True if a match.</returns>
        public bool IsMatch(Uri resourceUri, string accessType)
        {
            var resourceNameMatcher = new Matcher();
            resourceNameMatcher.AddInclude(this.Resource.Uri.ToString());
            PatternMatchingResult resourceNameMatchResult = resourceNameMatcher.Match(resourceUri.ToString());

            var accessTypeMatcher = new Matcher();
            accessTypeMatcher.AddInclude(this.AccessType);
            PatternMatchingResult accessTypeMatchResult = accessTypeMatcher.Match(accessType);

            return resourceNameMatchResult.HasMatches && accessTypeMatchResult.HasMatches;
        }
    }
}