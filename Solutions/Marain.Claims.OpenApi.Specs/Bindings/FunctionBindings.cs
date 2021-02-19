// <copyright file="WorkflowFunctionBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Claims.OpenApi.Specs.Bindings
{
    using System.Threading.Tasks;
    using Corvus.Testing.AzureFunctions;
    using Corvus.Testing.AzureFunctions.SpecFlow;
    using Corvus.Testing.SpecFlow;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using TechTalk.SpecFlow;

    /// <summary>
    /// SpecFlow bindings to run the workflow functions using the Functions tools.
    /// </summary>
    [Binding]
    public static class FunctionBindings
    {
        public const int ClaimsHostPort = 7076;

        /// <summary>
        /// Sets up and runs the function using the functions runtime.
        /// </summary>
        /// <param name="context">The current <see cref="FeatureContext"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [BeforeFeature("@useClaimsApi", Order = BindingSequence.FunctionStartup)]
        public static async Task StartClaimsFunctionAsync(FeatureContext context)
        {
            await FunctionsBindings.GetFunctionsController(context).StartFunctionsInstance(
                    "Marain.Claims.Host.Functions",
                    ClaimsHostPort,
                    "netcoreapp3.1",
                    "csharp",
                    FunctionsBindings.GetFunctionConfiguration(context));
        }

        [AfterScenario("useClaimsApi")]
        public static void WriteFunctionsOutput(FeatureContext featureContext)
        {
            ILogger logger = ContainerBindings.GetServiceProvider(featureContext).GetRequiredService<ILogger>();
            FunctionsController functionsController = FunctionsBindings.GetFunctionsController(featureContext);
            logger.LogAllAndClear(functionsController.GetFunctionsOutput());
        }

        /// <summary>
        /// Tear down the running functions instances for the scenario.
        /// </summary>
        /// <param name="context">The current <see cref="FeatureContext"/>.</param>
        [AfterFeature]
        public static void TeardownFunctionsAfterScenario(FeatureContext context)
        {
            if (context.TryGetValue(out FunctionsController controller))
            {
                context.RunAndStoreExceptions(controller.TeardownFunctions);
            }
        }
    }
}
