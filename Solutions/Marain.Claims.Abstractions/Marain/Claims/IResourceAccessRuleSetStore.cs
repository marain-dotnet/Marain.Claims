// <copyright file="IResourceAccessRuleSetStore.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    ///     An interface for classes that handle persistence of <see cref="ResourceAccessRuleSet" />.
    /// </summary>
    public interface IResourceAccessRuleSetStore
    {
        /// <summary>
        ///     Gets the specified <see cref="ResourceAccessRuleSet" /> from the
        ///     repository.
        /// </summary>
        /// <param name="id">
        ///     The id of the resource access rule set to retrieve.
        /// </param>
        /// <returns>
        ///     A <see cref="Task" /> that will return the <see cref="ResourceAccessRuleSet" />
        ///     when it completes.
        /// </returns>
        Task<ResourceAccessRuleSet> GetAsync(string id);

        /// <summary>
        ///     Gets the specified <see cref="ResourceAccessRuleSet" /> from the
        ///     repository.
        /// </summary>
        /// <param name="id">
        ///     The id of the resource access rule set to retrieve.
        /// </param>
        /// <param name="eTag">
        ///     The etag of the current version of the resource access ruleset.
        /// </param>
        /// <returns>
        ///     A <see cref="Task" /> that will return the <see cref="ResourceAccessRuleSet" />
        ///     when it completes. If the etag matched, then the rule set will not be downloaded, and <c>null</c> will be returned.
        /// </returns>
        Task<ResourceAccessRuleSet> GetAsync(string id, string eTag);

        /// <summary>
        ///     Gets the specified batch of <see cref="ResourceAccessRuleSet" /> from the
        ///     repository.
        /// </summary>
        /// <param name="ids">The ids of the rule sets to retrieve.</param>
        /// <param name="maxParallelism">The maximum parallelization of the request.</param>
        /// <returns>
        ///     A <see cref="Task" /> that will return the <see cref="ResourceAccessRuleSetCollection" />
        ///     when it completes.
        /// </returns>
        Task<ResourceAccessRuleSetCollection> GetBatchAsync(IEnumerable<string> ids, int maxParallelism = 5);

        /// <summary>
        ///     Gets the specified batch of <see cref="ResourceAccessRuleSet" /> from the
        ///     repository.
        /// </summary>
        /// <param name="ids">The ids of the rule sets to retrieve, along with the ETag of the current version of those IDs.</param>
        /// <param name="maxParallelism">The maximum parallelization of the request.</param>
        /// <returns>
        ///     A <see cref="Task" /> that will return the <see cref="ResourceAccessRuleSetCollection" />
        ///     when it completes.
        /// </returns>
        /// <remarks>
        /// This will only return those rulesets whose <see cref="ResourceAccessRuleSet.ETag"/> differed from those supplied with the ID.
        /// </remarks>
        Task<ResourceAccessRuleSetCollection> GetBatchAsync(IEnumerable<IdWithETag> ids, int maxParallelism = 5);

        /// <summary>
        ///     Saves or updates the given <see cref="ResourceAccessRuleSet" />.
        /// </summary>
        /// <param name="resourceAccessRuleSet">
        ///     The resource access rule set to save.
        /// </param>
        /// <returns>
        ///    A <see cref="Task" /> that will return the <see cref="ResourceAccessRuleSet" /> when the store has been updated.
        /// </returns>
        Task<ResourceAccessRuleSet> PersistAsync(ResourceAccessRuleSet resourceAccessRuleSet);
    }
}
