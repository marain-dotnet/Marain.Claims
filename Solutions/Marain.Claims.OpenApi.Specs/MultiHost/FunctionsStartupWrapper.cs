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
    using Microsoft.Extensions.DependencyInjection;

    public class FunctionsStartupWrapper : IWebJobsStartup
    {
        private readonly Startup wrappedStartup = new Startup();

        public void Configure(IWebJobsBuilder builder)
        {
            this.wrappedStartup.Configure(builder);

            // Add services normally automatically present in the Functions Host that we rely on.
            // TODO: remove once upgraded to Corvus.Monitoring v2, and we've taken out the telemetry code from ClaimPermissionsService
            builder.Services.AddSingleton(new TelemetryClient(new TelemetryConfiguration()));
        }
    }
}
