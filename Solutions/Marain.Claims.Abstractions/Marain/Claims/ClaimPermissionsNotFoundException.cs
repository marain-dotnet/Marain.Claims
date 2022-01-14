// <copyright file="ClaimPermissionsNotFoundException.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

#pragma warning disable IDE0079 // If you don't have Roslynator, the next suppression is considered redundant
#pragma warning disable RCS1194 // Roslynator's 'all the constructors' fixation
#pragma warning restore IDE0079

namespace Marain.Claims.Storage
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Thrown when a claim permissions with the specified id cannot be found.
    /// </summary>
    [Serializable]
    public class ClaimPermissionsNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimPermissionsNotFoundException"/> class.
        /// </summary>
        /// <param name="id">The id of the claim permissions that couldn't be found.</param>
        /// <param name="innerException">The inner exception.</param>
        public ClaimPermissionsNotFoundException(string id, Exception innerException)
            : base("Claim permissions not found", innerException)
        {
            this.ClaimPermissionsId = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimPermissionsNotFoundException"/> class.
        /// </summary>
        /// /// <param name="info"> The serialization info. </param>
        /// <param name="context"> The context. </param>
        protected ClaimPermissionsNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets the id of the <see cref="ClaimPermissions"/> that couldn't be found.
        /// </summary>
        public string ClaimPermissionsId { get; }
    }
}