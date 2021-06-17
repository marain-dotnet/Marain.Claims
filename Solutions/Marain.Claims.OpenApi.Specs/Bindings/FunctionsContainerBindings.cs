// <copyright file="WorkflowFunctionsContainerBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.Specs.Bindings
{
    using System;
    using Corvus.Identity.ManagedServiceIdentity.ClientAuthentication;
    using Corvus.Testing.SpecFlow;
    using Marain.Claims.Client;
    using Marain.Claims.OpenApi.Specs.Bindings;
    using Marain.Claims.OpenApi.Specs.MultiHost;
    using Marain.Services;
    using Marain.Tenancy.Client;

    using Microsoft.ApplicationInsights;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Bindings to set up the test container with workflow services so that test setup/teardown can be performed.
    /// </summary>
    [Binding]
    public static class FunctionsContainerBindings
    {
        /// <summary>
        /// Setup the endjin container for a feature.
        /// </summary>
        /// <param name="featureContext">The feature context for the current feature.</param>
        /// <remarks>We expect features run in parallel to be executing in separate app domains.</remarks>
        [BeforeFeature("@perFeatureContainer", Order = ContainerBeforeFeatureOrder.PopulateServiceCollection)]
        public static void SetupFeature(FeatureContext featureContext)
        {
            ContainerBindings.ConfigureServices(
                featureContext,
                services =>
                {
                    IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
                        .AddEnvironmentVariables()
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                    IConfiguration root = configurationBuilder.Build();

                    services.AddSingleton(root);
                    services.AddJsonSerializerSettings();

                    services.AddLogging();

                    string azureServicesAuthConnectionString = root["AzureServicesAuthConnectionString"];

                    services.AddAzureManagedIdentityBasedTokenSource(
                        new AzureManagedIdentityTokenSourceOptions
                        {
                            AzureServicesAuthConnectionString = azureServicesAuthConnectionString,
                        });

                    services.AddRootTenant();
                    services.AddSingleton(sp => sp.GetRequiredService<IConfiguration>().GetSection("TenancyClient").Get<TenancyClientOptions>());

                    TenancyClientOptions tenancyClientConfiguration = root.GetSection("TenancyClient").Get<TenancyClientOptions>();
                    services.AddSingleton(tenancyClientConfiguration);
                    services.AddTenantProviderServiceClient();

                    services.AddTenantedClaimsApi(root);
                    //// TODO: remove once upgraded to Corvus.Monitoring v2, and we've taken out the telemetry code from ClaimPermissionsService
                    services.AddSingleton(new TelemetryClient(new Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration()));

                    services.AddClaimsClient(_ =>
                    {
                        return new ClaimsClientOptions
                        {
                            BaseUri = new Uri($"http://localhost:{FunctionBindings.ClaimsHostPort}"),
                        };
                    });
                });
        }
    }
}