// <copyright file="CreatePet.feature.multi.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.OpenApi.Specs.Features
{
    using Marain.Claims.OpenApi.Specs.MultiHost;

    using NUnit.Framework;

    /// <summary>
    /// Adds in multi-form execution.
    /// </summary>
    [TestFixtureSource(nameof(FixtureArgs))]
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