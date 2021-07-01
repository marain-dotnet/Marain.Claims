// <copyright file="BindingSequence.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.OpenApi.Specs.Bindings
{
    using Corvus.Testing.SpecFlow;

    public static class BindingSequence
    {
        public const int TransientTenantSetup = ContainerBeforeFeatureOrder.ServiceProviderAvailable;

        public const int FunctionStartup = TransientTenantSetup + 1;

        public const int FunctionRunning = FunctionStartup + 1;

        public const int TransientTenantTearDown = FunctionStartup + 1;
    }
}
