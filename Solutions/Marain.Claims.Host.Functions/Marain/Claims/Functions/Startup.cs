// <copyright file="Startup.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

[assembly: Microsoft.Azure.WebJobs.Hosting.WebJobsStartup(typeof(Marain.Operations.ControlHost.Startup))]

namespace Marain.Operations.ControlHost
{
    /// <summary>
    /// Startup code for the Function.
    /// </summary>
    public class Startup : IWebJobsStartup
    {
        /// <inheritdoc/>
        public void Configure(IWebJobsBuilder builder)
        {
            IServiceCollection services = builder.Services;

            IConfigurationRoot root = Configure(services);

            services.AddLogging(logging =>
            {
#if DEBUG
                // Ensure you enable the required logging level in host.json
                // e.g:
                //
                // "logging": {
                //    "fileLoggingMode": "debugOnly",
                //    "logLevel": {
                //
                //    // For all functions
                //    "Function": "Debug",
                //
                //    // Default settings, e.g. for host
                //    "default": "Debug"
                // }
                logging.AddConsole();
#endif
            });

            services.AddTenantedClaimsApi(root, config => config.Documents.AddSwaggerEndpoint());
        }

        private static IConfigurationRoot Configure(IServiceCollection services)
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot root = configurationBuilder.Build();
            services.AddSingleton(root);
            return root;
        }
    }
}
