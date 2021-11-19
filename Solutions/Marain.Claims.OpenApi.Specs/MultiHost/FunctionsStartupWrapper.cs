// <copyright file="WorkflowFunctionsStartupWrapper.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.OpenApi.Specs.MultiHost
{
    using Marain.Claims.Functions;

    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public class FunctionsStartupWrapper : IWebJobsStartup
    {
        private readonly Startup wrappedStartup = new Startup();
        private IConfiguration configuration;

        public FunctionsStartupWrapper(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void Configure(IWebJobsBuilder builder)
        {
            var context = new WebJobsBuilderContext
            {
                Configuration = this.configuration,
            };
            builder.Services.AddLogging(config =>
                config.AddConsole(opt => opt.LogToStandardErrorThreshold = LogLevel.Debug));
            this.wrappedStartup.Configure(context, builder);

            // Add services normally automatically present in the Functions Host that we rely on.
            // TODO: we were planning to remove thisd once upgraded to Corvus.Monitoring v2, but for some
            // reason the tests fail without this. Need to work out why.
            builder.Services.AddSingleton(new TelemetryClient(new TelemetryConfiguration()));
        }
    }
}
