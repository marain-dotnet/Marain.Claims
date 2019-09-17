// <copyright file="Program.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

namespace Marain.Claims.SetupTool
{
    using System;
    using System.Collections.Generic;
    using System.CommandLine;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Marain.Claims.SetupTool.Commands;
    using Endjin.Cli;
    using Endjin.Composition;
    using Microsoft.Extensions.DependencyInjection;

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
            ConfigureContainer();

            var result = ArgumentSyntax.Parse(args, syntax =>
            {
                syntax.ApplicationName = "ClaimsSetup";
                IEnumerable<ICommand> commands = ServiceRoot.ServiceProvider.GetServices<ICommand>();
                commands.ForEach(command => command.Add(syntax));
            });

            await ((ICommand)result.ActiveCommand.Value).ExecuteAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Configures the IoC container.
        /// </summary>
        private static void ConfigureContainer()
        {
            var services = new ServiceCollection();

            IEnumerable<Type> commands = Assembly
                .GetExecutingAssembly()
                .GetExportedTypes()
                .Where(x => x.Namespace == typeof(ShowAppIdentityInformation).Namespace && typeof(ICommand).IsAssignableFrom(x));
            commands.ForEach(x => services.AddTransient(typeof(ICommand), x));

            services.AddSingleton<AppServiceManagerSource>();

            services.BuildServices();
        }
    }
}
