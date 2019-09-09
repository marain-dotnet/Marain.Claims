// <copyright file="IPermissionsStoreFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims
{
    using System.Threading.Tasks;
    using Corvus.Tenancy;

    /// <summary>
    ///     Factory class for obtaining instances of <see cref="IClaimPermissionsStore" />
    ///     and <see cref="IResourceAccessRuleSetStore" />.
    /// </summary>
    public interface IPermissionsStoreFactory
    {
        /// <summary>
        ///     Gets the <see cref="IClaimPermissionsStore" />.
        /// </summary>
        /// <param name="tenant">
        ///     The tenant.
        /// </param>
        /// <returns>
        ///     The <see cref="IClaimPermissionsStore" />.
        /// </returns>
        Task<IClaimPermissionsStore> GetClaimPermissionsStoreAsync(ITenant tenant);

        /// <summary>
        ///     Gets the <see cref="IResourceAccessRuleSetStore" />.
        /// </summary>
        /// <param name="tenant">
        ///     The tenant.
        /// </param>
        /// <returns>
        ///     The <see cref="IResourceAccessRuleSetStore" />.
        /// </returns>
        Task<IResourceAccessRuleSetStore> GetResourceAccessRuleSetStoreAsync(ITenant tenant);
    }
}