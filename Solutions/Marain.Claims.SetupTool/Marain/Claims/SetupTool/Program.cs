// <copyright file="Program.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.SetupTool
{
    using System.Threading.Tasks;
    using Corvus.Cli;
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
        private static async Task Main(string[] args)
        {
            var application = new CommandLineApplication
            {
                Name = "ClaimsSetup",
                Description = "Setup claims for a tenant.",
            };

            application.AddCommand<BootstrapClaimsTenant>();
            application.AddCommand<DefineRulesetsAndClaimPermissions>();
            application.AddCommand<ShowAppIdentityInformation>();

            await application.ExecuteAsync(args);
        }
    }
}
