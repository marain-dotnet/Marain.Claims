// <copyright file="IMultiModeTest.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.OpenApi.Specs.MultiHost
{
    /// <summary>
    /// Supports executing the same test fixture multiple times in different modes.
    /// </summary>
    /// <typeparam name="T">The type used to indicate the mode.</typeparam>
    internal interface IMultiModeTest<T>
    {
        /// <summary>
        /// Gets the mode in which the test is executing.
        /// </summary>
        T TestType { get; }
    }
}