// <copyright file="ClaimPermissionsModifyRules.feature.multi.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.OpenApi.Specs.Features
{
    using Marain.Claims.OpenApi.Specs.MultiHost;

    /// <summary>
    /// Adds in multi-host-mode execution.
    /// </summary>
    [MultiHostTest]
    public partial class ClaimPermissionsFeature : MultiTestHostBase
    {
        /// <summary>
        /// Creates a <see cref="ClaimPermissionsFeature"/>.
        /// </summary>
        /// <param name="hostMode">
        /// Hosting style to test for.
        /// </param>
        public ClaimPermissionsFeature(TestHostModes hostMode)
            : base(hostMode)
        {
        }
    }
}