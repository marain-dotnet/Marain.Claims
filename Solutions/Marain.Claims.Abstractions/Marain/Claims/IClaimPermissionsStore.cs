// <copyright file="IClaimPermissionsStore.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    ///     An interface for classes that handle persistence of <see cref="ClaimPermissions" />.
    /// </summary>
    public interface IClaimPermissionsStore
    {
        /// <summary>
        ///     Gets the specified <see cref="ClaimPermissions" /> from the
        ///     repository.
        /// </summary>
        /// <param name="id">
        ///     The id of the claim permissions to retrieve.
        /// </param>
        /// <returns>
        ///     A <see cref="Task" /> that will return the <see cref="ClaimPermissions" />
        ///     when it completes.
        /// </returns>
        Task<ClaimPermissions> GetAsync(string id);

        /// <summary>
        ///     Gets the specified batch of <see cref="ClaimPermissions" /> from the
        ///     repository.
        /// </summary>
        /// <param name="ids">The ids of the permissions to retrieve.</param>
        /// <param name="maxParallelism">The maximum parallelization of the request.</param>
        /// <returns>
        ///     A <see cref="Task" /> that will return the <see cref="ClaimPermissionsCollection" />
        ///     when it completes.
        /// </returns>
        Task<ClaimPermissionsCollection> GetBatchAsync(IEnumerable<string> ids, int maxParallelism = 5);

        /// <summary>
        ///     Persists a new <see cref="ClaimPermissions" />.
        /// </summary>
        /// <param name="claimPermissions">
        ///     The claim permissions to create.
        /// </param>
        /// <returns>
        ///     A <see cref="Task" /> that will complete when the store has been updated, and which
        ///     returns a <see cref="ClaimPermissions"/> with the ETag populated.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown if a <see cref="ClaimPermissions"/> with the specified ID already exists.
        /// </exception>
        Task<ClaimPermissions> CreateAsync(ClaimPermissions claimPermissions);

        /// <summary>
        ///     Persists a modified version of an existing <see cref="ClaimPermissions" />.
        /// </summary>
        /// <param name="claimPermissions">
        ///     The claim permissions to create.
        /// </param>
        /// <returns>
        ///     A <see cref="Task" /> that will complete when the store has been updated.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        ///     Thrown if the <see cref="ClaimPermissions"/> passed has a null ETag property.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        ///     Thrown if a <see cref="ClaimPermissions"/> the persisted copy does not match the ETag,
        ///     indicating that updates might be lost of the change is saved.
        /// </exception>
        Task<ClaimPermissions> UpdateAsync(ClaimPermissions claimPermissions);

        /// <summary>
        ///     Determines whether there are any claim permissions in the store.
        /// </summary>
        /// <returns>
        ///     A <see cref="Task"/> that produces a <c>true</c> result if at least one claim
        ///     permissions exists in the store, and <c>false</c> if the store contains none.
        /// </returns>
        Task<bool> AnyPermissions();
    }
}