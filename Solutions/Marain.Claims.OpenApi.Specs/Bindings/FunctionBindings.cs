// <copyright file="WorkflowFunctionBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.OpenApi.Specs.Bindings
{
    using System;
    using System.Text;
    using System.Threading.Tasks;

    using BoDi;

    using Corvus.Extensions.Json;
    using Corvus.Testing.AzureFunctions;
    using Corvus.Testing.AzureFunctions.SpecFlow;
    using Corvus.Testing.SpecFlow;

    using Marain.Claims.OpenApi.Specs.MultiHost;
    using Marain.Services;

    using Menes.Testing.AspNetCoreSelfHosting;

    using Microsoft.Extensions.DependencyInjection;

    using NUnit.Framework.Internal;

    using TechTalk.SpecFlow;

    /// <summary>
    /// SpecFlow bindings to run the workflow functions using the Functions tools.
    /// </summary>
    [Binding]
    public static class FunctionBindings
    {
        public const int ClaimsHostPort = 7076;

        private static readonly string ServiceUrl = $"http://localhost:{ClaimsHostPort}";

        public static TestHostModes TestHostMode => TestExecutionContext.CurrentContext.TestObject switch
        {
            IMultiModeTest<TestHostModes> multiModeTest => multiModeTest.TestType,
            _ => TestHostModes.UseFunctionHost,
        };

        /// <summary>
        /// Sets up and runs the function using the functions runtime.
        /// </summary>
        /// <param name="context">The current <see cref="FeatureContext"/>.</param>
        /// <param name="testTenants">Provides access to the transient tenants created for this test.</param>
        /// <param name="specFlowDiContainer">Enables us to add objects that can be injected into bindings.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [BeforeFeature("@useClaimsApi", Order = BindingSequence.FunctionStartup)]
        public static async Task StartClaimsServiceAsync(
            FeatureContext context,
            ClaimsServiceTestTenants testTenants,
            IObjectContainer specFlowDiContainer)
        {
            switch (TestHostMode)
            {
                case TestHostModes.InProcessEmulateFunctionWithActionResult:
                    var hostManager = new OpenApiWebHostManager();
                    context.Set(hostManager);
                    await hostManager.StartHostAsync<FunctionsStartupWrapper>(
                        ServiceUrl,
                        services =>
                        {
                            services.AddSingleton(new MarainServiceConfiguration
                            {
                                ServiceDisplayName = "Claims v1",
                                ServiceTenantId = testTenants.TransientServiceTenantId,
                            });
                        });
                    break;

                case TestHostModes.UseFunctionHost:
                    // Setting MarainServiceConfiguration:ServiceTenantId doesn't work environment variables we pass
                    // here will be overridden by ones set in the local.settings.json. We want local.settings.json to
                    // be able to contain a value for this because that's useful for debugging the service
                    // directly, but tests need to override it because we generate a temporary service tenant
                    // in order to isolate the test.
                    FunctionConfiguration functionConfiguration = FunctionsBindings.GetFunctionConfiguration(context);
                    functionConfiguration.EnvironmentVariables["MarainServiceConfiguration__ServiceTenantIdOverride"] = testTenants.TransientServiceTenantId;
                    await FunctionsBindings.GetFunctionsController(context).StartFunctionsInstance(
                            "Marain.Claims.Host.Functions",
                            ClaimsHostPort,
                            "netcoreapp3.1",
                            "csharp",
                            functionConfiguration);
                    break;
            }

            IServiceProvider serviceProvider = ContainerBindings.GetServiceProvider(context);
            ITestableClaimsService serviceWrapper = TestHostMode == TestHostModes.DirectInvocation
                ? new DirectTestableClaimsService(
                    testTenants,
                    serviceProvider.GetRequiredService<ClaimPermissionsService>(),
                    serviceProvider.GetRequiredService<ResourceAccessRuleSetService>())
                : new ClientTestableClaimsService(
                    testTenants,
                    ServiceUrl,
                    serviceProvider.GetRequiredService<IJsonSerializerSettingsProvider>().Instance);

            specFlowDiContainer.RegisterInstanceAs(serviceWrapper);
        }

        [BeforeFeature("@useClaimsApi", Order = BindingSequence.FunctionRunning)]
        public static async Task StartClaimsServiceAsync(
            FeatureContext context,
            ITestableClaimsService serviceWrapper)
        {
            try
            {
                // To be able to test the Claims service, the claims configuration for the Claims service
                // needs to be in place. (Claims uses itself to protect itself.) Since each test feature run
                // creates new service and client tenants, we need to bootstrap this inital claims configuration
                // every time.
                await serviceWrapper.BootstrapTenantClaimsPermissions();
            }
            catch (Exception x)
            {
                // To be able to see failures during startup, the only real way to do that seems to be to put
                // all the text we want in an exception message. Sigh.
                var sb = new StringBuilder();
                sb.AppendLine("Failed: " + x);

                FunctionsController functionsController = FunctionsBindings.GetFunctionsController(context);

                foreach (IProcessOutput po in functionsController.GetFunctionsOutput())
                {
                    sb.AppendLine(po.StandardOutputText);
                    sb.AppendLine(po.StandardErrorText);
                }

                throw new Exception(sb.ToString());
            }
        }

        [AfterScenario("useClaimsApi")]
        public static void WriteFunctionsOutput(FeatureContext featureContext)
        {
            if (TestHostMode == TestHostModes.UseFunctionHost)
            {
                FunctionsController functionsController = FunctionsBindings.GetFunctionsController(featureContext);
                foreach (IProcessOutput po in functionsController.GetFunctionsOutput())
                {
                    Console.WriteLine(po.StandardOutputText);
                    Console.WriteLine(po.StandardErrorText);

                    po.ClearAllOutput();
                }
            }
        }

        /// <summary>
        /// Tear down the running functions instances for the scenario.
        /// </summary>
        /// <param name="context">The current <see cref="FeatureContext"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [AfterFeature]
        public static async Task TeardownFunctionsAfterScenarioAsync(FeatureContext context)
        {
            if (TestHostMode == TestHostModes.UseFunctionHost)
            {
                if (context.TryGetValue(out FunctionsController controller))
                {
                    context.RunAndStoreExceptions(controller.TeardownFunctions);
                }
            }
            else if (TestHostMode == TestHostModes.InProcessEmulateFunctionWithActionResult)
            {
                if (context.TryGetValue(out OpenApiWebHostManager hostManager))
                {
                    await hostManager.StopAllHostsAsync();
                }
            }
        }
    }
}
