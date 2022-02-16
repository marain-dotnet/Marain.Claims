// <copyright file="Program.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.SetupTool
{
    using System.Threading.Tasks;
    using Marain.Claims.SetupTool.Commands;
    using McMaster.Extensions.CommandLineUtils;

    /// <summary>
    /// Program entry point.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Program entry point.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        private static Task<int> Main(string[] args)
        {
            return CommandLineApplication.ExecuteAsync<ClaimsSetup>(args);
        }
    }
}