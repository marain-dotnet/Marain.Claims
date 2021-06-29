// <copyright file="ClaimPermissionsModifyRuleSets.feature.multi.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.OpenApi.Specs.Features
{
    using Marain.Claims.OpenApi.Specs.MultiHost;

    /// <summary>
    /// Adds in multi-host-mode execution.
    /// </summary>
    [MultiHostTest]
    public partial class ClaimPermissionsModifyRuleSetsFeature : MultiTestHostBase
    {
        /// <summary>
        /// Creates a <see cref="ClaimPermissionsModifyRuleSetsFeature"/>.
        /// </summary>
        /// <param name="hostMode">
        /// Hosting style to test for.
        /// </param>
        public ClaimPermissionsModifyRuleSetsFeature(TestHostModes hostMode)
            : base(hostMode)
        {
        }
    }
}