// <copyright file="ClaimsContainerBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.SpecFlow.Bindings
{
    using System.Collections.Generic;
    using Corvus.Azure.Storage.Tenancy;
    using Corvus.Testing.SpecFlow;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;

    using TechTalk.SpecFlow;

    /// <summary>
    ///     Container related bindings to configure the service provider for features.
    /// </summary>
    [Binding]
    public static class ClaimsContainerBindings
    {
        /// <summary>
        /// Initializes the container before each feature's tests are run.
        /// </summary>
        /// <param name="featureContext">The SpecFlow test context.</param>
        [BeforeFeature("@perFeatureContainer", Order = ContainerBeforeFeatureOrder.PopulateServiceCollection)]
        public static void InitializeContainer(FeatureContext featureContext)
        {
            ContainerBindings.ConfigureServices(
                featureContext,
                serviceCollection =>
                {
                    IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
                        .AddEnvironmentVariables()
                        .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);

                    IConfiguration root = configurationBuilder.Build();

                    string azureServicesAuthConnectionString = root["AzureServicesAuthConnectionString"];

                    serviceCollection.AddSingleton(root);

                    serviceCollection.AddLogging();

                    serviceCollection.AddInMemoryTenantProvider();

                    serviceCollection.AddJsonNetSerializerSettingsProvider();
                    serviceCollection.AddJsonNetPropertyBag();
                    serviceCollection.AddJsonNetCultureInfoConverter();
                    serviceCollection.AddJsonNetDateTimeOffsetToIso8601AndUnixTimeConverter();
                    serviceCollection.AddSingleton<JsonConverter>(new StringEnumConverter(new CamelCaseNamingStrategy()));

                    var tenantCloudBlobContainerFactoryOptions = new TenantCloudBlobContainerFactoryOptions
                    {
                        AzureServicesAuthConnectionString = azureServicesAuthConnectionString,
                    };

                    serviceCollection.AddSingleton(tenantCloudBlobContainerFactoryOptions);

                    serviceCollection.AddTenantCloudBlobContainerFactory(tenantCloudBlobContainerFactoryOptions);

                    serviceCollection.AddTenantedBlobContainerClaimsStore();

                    serviceCollection.AddMarainServiceConfiguration();
                    serviceCollection.AddMarainServicesTenancy();
                });
        }
    }
}
