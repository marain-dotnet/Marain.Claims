// <copyright file="Resource.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims
{
    using System;

    /// <summary>
    /// Struct representing a resource, or a set of resources.
    /// </summary>
    /// <remarks>
    /// <para>
    /// You can define a resource either as an actual value, e.g.
    /// </para>
    /// <code>
    /// new Resource(new Uri("page/home", UriKind.Relative), "Home page"));
    /// </code>
    /// <para>
    /// or using a globbing pattern, e.g.
    /// </para>
    /// <code>
    /// new Resource(new Uri("books/**", UriKind.Relative), "All books"));
    /// </code>
    /// </remarks>
    public struct Resource : IEquatable<Resource>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Resource"/> struct.
        /// </summary>
        /// <param name="uri">The unique identifier of the resource, or a pattern using either * or **.</param>
        /// <param name="displayName">The display name to assign for the resource.</param>
        public Resource(Uri uri, string displayName)
            : this()
        {
            this.Uri = uri;
            this.DisplayName = displayName;
        }

        /// <summary>
        /// Gets the unique identifier for the resource. The Uri should be relative, and can either be
        /// an actual value (e.g. "page/home") or a globbing pattern (e.g. "books/**").
        /// </summary>
        /// <remarks>
        /// Supports globbing: use '/' to separate segments, use '*' for wildcard in segment,
        /// use '**' for arbitrary segment depth, use '..' for parent segment.
        /// </remarks>
        public Uri Uri { get; }

        /// <summary>
        /// Gets the display name for the resource.
        /// </summary>
        /// <remarks>
        /// The display name is ignored for comparison purposes. Two <see cref="Resource"/> values
        /// are considered equal if they have the same <see cref="Uri"/>.
        /// </remarks>
        public string DisplayName { get; }

        /// <summary>
        /// Compares two <see cref="Resource"/> values.
        /// </summary>
        /// <param name="x">Left <see cref="Resource"/> to compare.</param>
        /// <param name="y">Right <see cref="Resource"/> to compare.</param>
        /// <returns>True if left and right <see cref="Resource"/> are equal.</returns>
        public static bool operator ==(Resource x, Resource y) => x.Equals(y);

        /// <summary>
        /// Compares two <see cref="Resource"/> values.
        /// </summary>
        /// <param name="x">Left <see cref="Resource"/> to compare.</param>
        /// <param name="y">Right <see cref="Resource"/> to compare.</param>
        /// <returns>True if left and right <see cref="Resource"/> are not equal.</returns>
        public static bool operator !=(Resource x, Resource y) => !x.Equals(y);

        /// <summary>
        /// Get a hash code for the object, based on the <see cref="Uri"/>.
        /// (<see cref="DisplayName"/> is ignored for comparison purposes.)
        /// </summary>
        /// <returns>A hash code.</returns>
        public override int GetHashCode()
        {
            return this.Uri.ToString().ToLowerInvariant().GetHashCode();
        }

        /// <summary>
        /// Compares this <see cref="Resource"/> with another.
        /// </summary>
        /// <param name="obj">The other <see cref="Resource"/> to compare.</param>
        /// <returns>True if left and right <see cref="Resource"/> are equal.</returns>
        public override bool Equals(object obj)
        {
            return obj is Resource res && this.Equals(res);
        }

        /// <summary>
        /// Compares this <see cref="Resource"/> with another.
        /// </summary>
        /// <param name="other">The other <see cref="Resource"/> to compare.</param>
        /// <returns>True if left and right <see cref="Resource"/> are equal.</returns>
        public bool Equals(Resource other)
        {
            return
                Uri.Compare(this.Uri, other.Uri, UriComponents.AbsoluteUri, UriFormat.Unescaped, StringComparison.InvariantCultureIgnoreCase) == 0;
        }
    }
}