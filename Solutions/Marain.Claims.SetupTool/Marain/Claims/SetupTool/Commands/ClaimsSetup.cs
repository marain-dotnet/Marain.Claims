// <copyright file="ClaimsSetup.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.SetupTool.Commands
{
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;

    /// <summary>
    /// The claims setup application.
    /// </summary>
    [Command(FullName = "ClaimsSetup", Description = "Setup claims for a tenant.", ShowInHelpText = true)]
    [HelpOption]
    [Subcommand(typeof(BootstrapClaimsTenant))]
    [Subcommand(typeof(DefineRulesetsAndClaimPermissions))]
    [Subcommand(typeof(ShowAppIdentityInformation))]
    public class ClaimsSetup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClaimsSetup"/> class.
        /// </summary>
        public ClaimsSetup()
        {
        }

#pragma warning disable CA1822 // Mark members as static - invoked through reflection
        private Task<int> OnExecuteAsync(CommandLineApplication app, IConsole console)
#pragma warning restore CA1822
        {
            app.ShowHelp();
            return Task.FromResult(1);
        }
    }
}
