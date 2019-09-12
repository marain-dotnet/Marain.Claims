// <copyright file="Startup.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

[assembly: Microsoft.Azure.WebJobs.Hosting.WebJobsStartup(typeof(Marain.Operations.ControlHost.Startup))]

namespace Marain.Operations.ControlHost
{
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Serilog;
    using Serilog.Filters;

    /// <summary>
    /// Startup code for the Function.
    /// </summary>
    public class Startup : IWebJobsStartup
    {
        /// <inheritdoc/>
        public void Configure(IWebJobsBuilder builder)
        {
            IServiceCollection services = builder.Services;

            LoggerConfiguration loggerConfig = new LoggerConfiguration()
                    .Enrich.FromLogContext()
                    .MinimumLevel.Debug()
                    .WriteTo.Logger(lc => lc.Filter.ByExcluding(Matching.FromSource("Menes")).WriteTo.Console().MinimumLevel.Debug())
                    .WriteTo.Logger(lc => lc.Filter.ByIncludingOnly(Matching.FromSource("Menes")).WriteTo.Console().MinimumLevel.Debug());

            Log.Logger = loggerConfig.CreateLogger();

            services.AddLogging();

            IConfigurationRoot root = Configure(services);

            services.AddTenantedOperationsControlApi(root, config => config.Documents.AddSwaggerEndpoint());
        }

        private static IConfigurationRoot Configure(IServiceCollection services)
        {
            var configurationBuilder = new ConfigurationBuilder();

            // Use local.settings if available (not available in Azure)
            configurationBuilder.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);

            // Then use the regular Azure environment variables
            configurationBuilder.AddEnvironmentVariables();

            IConfigurationRoot root = configurationBuilder.Build();
            services.AddSingleton(root);
            return root;
        }
    }
}
