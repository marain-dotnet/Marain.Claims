// <copyright file="WorkflowFunctionsContainerBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.OpenApi.Specs.Bindings
{
    using System;
    using System.Text;

    using Corvus.Identity.ManagedServiceIdentity.ClientAuthentication;
    using Corvus.Testing.SpecFlow;

    using Marain.Claims.Client;
    using Marain.Claims.OpenApi.Specs.Bindings;
    using Marain.Tenancy.Client;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;

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
                    services.AddJsonNetSerializerSettingsProvider();
                    services.AddJsonNetPropertyBag();
                    services.AddJsonNetCultureInfoConverter();
                    services.AddJsonNetDateTimeOffsetToIso8601AndUnixTimeConverter();
                    services.AddSingleton<JsonConverter>(new StringEnumConverter(new CamelCaseNamingStrategy()));

                    var logCollector = new LogProvider();
                    services.AddSingleton(logCollector);
                    services.AddLogging(config => config
                        .AddProvider(logCollector)
                        ////.AddConsole(opt => opt.LogToStandardErrorThreshold = LogLevel.Debug)
                        );

                    string azureServicesAuthConnectionString = root["AzureServicesAuthConnectionString"];

#pragma warning disable CS0618 // Type or member is obsolete
                    services.AddAzureManagedIdentityBasedTokenSource(
                        new AzureManagedIdentityTokenSourceOptions
                        {
                            AzureServicesAuthConnectionString = azureServicesAuthConnectionString,
                        });
#pragma warning restore CS0618 // Type or member is obsolete
                    services.AddServiceIdentityAzureTokenCredentialSourceFromLegacyConnectionString(azureServicesAuthConnectionString);

                    services.AddSingleton(sp => sp.GetRequiredService<IConfiguration>().GetSection("TenancyClient").Get<TenancyClientOptions>());

                    TenancyClientOptions tenancyClientConfiguration = root.GetSection("TenancyClient").Get<TenancyClientOptions>();
                    services.AddSingleton(tenancyClientConfiguration);
                    services.AddTenantProviderServiceClient();

                    services.AddTenantedClaimsApiWithOpenApiActionResultHosting(root);

                    services.AddClaimsClient(_ =>
                    {
                        return new ClaimsClientOptions
                        {
                            BaseUri = new Uri($"http://localhost:{FunctionBindings.ClaimsHostPort}"),
                        };
                    });
                });
        }

        public class LogProvider : ILoggerProvider
        {
            public StringBuilder Output { get; } = new StringBuilder();

            public ILogger CreateLogger(string categoryName)
            {
                return new Logger(this);
            }

            public void Dispose()
            {
            }

            private class Logger : ILogger
            {
                private LogProvider logProvider;

                public Logger(LogProvider logProvider)
                {
                    this.logProvider = logProvider;
                }

                public IDisposable BeginScope<TState>(TState state)
                {
                    return new Scope();
                }

                public bool IsEnabled(LogLevel logLevel)
                {
                    return true;
                }

                public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
                {
                    this.logProvider.Output.AppendLine(
                        $"[{logLevel}] [{eventId}], {formatter(state, exception)}");
                }

                private class Scope : IDisposable
                {
                    public void Dispose()
                    {
                    }
                }
            }
        }
    }
}