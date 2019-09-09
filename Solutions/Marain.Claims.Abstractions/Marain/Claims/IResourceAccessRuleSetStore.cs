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
