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
    public partial class ClaimPermissionsModifyRulesFeature : MultiTestHostBase
    {
        /// <summary>
        /// Creates a <see cref="ClaimPermissionsModifyRulesFeature "/>.
        /// </summary>
        /// <param name="hostMode">
        /// Hosting style to test for.
        /// </param>
        public ClaimPermissionsModifyRulesFeature(TestHostModes hostMode)
            : base(hostMode)
        {
        }
    }
}