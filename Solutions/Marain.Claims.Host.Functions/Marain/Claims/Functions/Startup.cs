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

            IConfigurationRoot root = Configure(services);

            services.AddTenantedClaimsApi(root, config => config.Documents.AddSwaggerEndpoint());
        }

        private static IConfigurationRoot Configure(IServiceCollection services)
        {
            var configurationBuilder = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot root = configurationBuilder.Build();
            services.AddSingleton(root);
            return root;
        }
    }
}
