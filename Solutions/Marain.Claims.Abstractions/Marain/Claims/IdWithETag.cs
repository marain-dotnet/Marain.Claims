// <copyright file="IdWithETag.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims
{
    using System;

    /// <summary>
    /// A tuple of ID and ETag.
    /// </summary>
    public struct IdWithETag : IEquatable<IdWithETag>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IdWithETag"/> struct.
        /// </summary>
        /// <param name="id">The ID.</param>
        /// <param name="eTag">The associated ETag.</param>
        public IdWithETag(string id, string eTag)
            : this()
        {
            this.Id = id ?? throw new System.ArgumentNullException(nameof(id));
            this.ETag = eTag;
        }

        /// <summary>
        /// Gets the ID.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the associated ETag.
        /// </summary>
        public string ETag { get; }

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="left">The left hand side of the comparison.</param>
        /// <param name="right">The right hand side of the comparison.</param>
        /// <returns>True if both the <see cref="ETag"/> and the <see cref="Id"/> match.</returns>
        public static bool operator ==(IdWithETag left, IdWithETag right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="left">The left hand side of the comparison.</param>
        /// <param name="right">The right hand side of the comparison.</param>
        /// <returns>True if either the <see cref="ETag"/> or the <see cref="Id"/> do not match.</returns>
        public static bool operator !=(IdWithETag left, IdWithETag right)
        {
            return !(left == right);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is IdWithETag tag && this.Equals(tag);
        }

        /// <inheritdoc/>
        public bool Equals(IdWithETag other)
        {
            return this.Id == other.Id &&
                   this.ETag == other.ETag;
        }

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(this.Id, this.ETag);
    }
}