// <copyright file="TestHostTypes.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.OpenApi.Specs.MultiHost
{
    /// <summary>
    /// Service hosting mechanisms for which test suites may be executed.
    /// </summary>
    /// <remarks>
    /// Menes supports two modes of ASP.NET Core hosting, and we want to run certain sets of tests
    /// against each of these. This enumeration type is used to determine which mode a suite is
    /// being executed for.
    /// TODO: when we upgrade to Menes 3, this will need an InProcessAspNetDirectPipeline entry.
    /// </remarks>
    public enum TestHostModes
    {
        /// <summary>
        /// Instantiate the target service type directly without going through Menes
        /// </summary>
        DirectInvocation,

        /// <summary>
        /// Host the service in-process via Menes, simulating the way in which it would be invoked
        /// when hosted in an Azure Function.
        /// </summary>
        InProcessEmulateFunctionWithActionResult,

        /// <summary>
        /// Host the service out of process using the function host emulator.
        /// </summary>
        UseFunctionHost,
    }
}