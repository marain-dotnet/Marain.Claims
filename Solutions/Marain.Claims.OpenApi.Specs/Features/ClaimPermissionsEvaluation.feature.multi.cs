// <copyright file="ClaimPermissionsEvaluation.feature.multi.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.OpenApi.Specs.Features
{
    using Marain.Claims.OpenApi.Specs.MultiHost;

    /// <summary>
    /// Adds in multi-host-mode execution.
    /// </summary>
    [MultiHostTest]
    public partial class ClaimPermissionsEvaluationFeature : MultiTestHostBase
    {
        /// <summary>
        /// Creates a <see cref="ClaimPermissionsEvaluationFeature"/>.
        /// </summary>
        /// <param name="hostMode">
        /// Hosting style to test for.
        /// </param>
        public ClaimPermissionsEvaluationFeature(TestHostModes hostMode)
            : base(hostMode)
        {
        }
    }
}