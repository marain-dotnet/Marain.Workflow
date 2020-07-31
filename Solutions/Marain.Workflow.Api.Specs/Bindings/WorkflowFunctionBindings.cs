// <copyright file="WorkflowFunctionBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.Specs.Bindings
{
    using System.Threading.Tasks;
    using Corvus.Testing.AzureFunctions;
    using Corvus.Testing.AzureFunctions.SpecFlow;
    using Corvus.Testing.SpecFlow;
    using NUnit.Framework;
    using TechTalk.SpecFlow;

    /// <summary>
    /// SpecFlow bindings to run the workflow functions using the Functions tools.
    /// </summary>
    [Binding]
    public static class WorkflowFunctionBindings
    {
#pragma warning disable SA1401 // Fields should be private
        /// <summary>
        /// The base Url that can be used to access the engine host API.
        /// </summary>
        public static string EngineHostBaseUrl = "http://localhost:" + EngineHostPort;

        /// <summary>
        /// The base Url that can be used to access the engine host API.
        /// </summary>
        public static string QueryHostBaseUrl = "http://localhost:" + QueryHostPort;

        /// <summary>
        /// The base Url that can be used to access the engine host API.
        /// </summary>
        public static string MessageProcessingHostBaseUrl = "http://localhost:" + MessageProcessingHostPort;
#pragma warning restore SA1401 // Fields should be private

        private const int EngineHostPort = 8765;
        private const int QueryHostPort = 8767;
        private const int MessageProcessingHostPort = 8766;

        /// <summary>
        /// Sets up and runs the function using the functions runtime.
        /// </summary>
        /// <param name="context">The current <see cref="FeatureContext"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [BeforeFeature("@useWorkflowEngineApi", Order = BindingSequence.FunctionStartup)]
        public static Task StartWorkflowEngineFunctionAsync(FeatureContext context)
        {
            return FunctionsBindings.GetFunctionsController(context).StartFunctionsInstance(
                    "Marain.Workflow.Api.EngineHost",
                    EngineHostPort,
                    "netcoreapp3.1",
                    "csharp",
                    FunctionsBindings.GetFunctionConfiguration(context));
        }

        /// <summary>
        /// Sets up and runs the function using the functions runtime.
        /// </summary>
        /// <param name="context">The current <see cref="FeatureContext"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [BeforeFeature("@useWorkflowMessageProcessingApi", Order = BindingSequence.FunctionStartup)]
        public static Task StartWorkflowMessageProcessingFunctionAsync(FeatureContext context)
        {
            FunctionConfiguration config = FunctionsBindings.GetFunctionConfiguration(context);
            config.EnvironmentVariables.Add("Workflow:EngineClient:BaseUrl", EngineHostBaseUrl);

            return FunctionsBindings.GetFunctionsController(context).StartFunctionsInstance(
                "Marain.Workflow.Api.MessageProcessingHost",
                MessageProcessingHostPort,
                "netcoreapp3.1",
                "csharp",
                config);
        }

        /// <summary>
        /// Sets up and runs the function using the functions runtime.
        /// </summary>
        /// <param name="context">The current <see cref="FeatureContext"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [BeforeFeature("@useWorkflowQueryApi", Order = BindingSequence.FunctionStartup)]
        public static Task StartWorkflowQueryFunctionAsync(FeatureContext context)
        {
            return FunctionsBindings.GetFunctionsController(context).StartFunctionsInstance(
                "Marain.Workflow.Api.QueryHost",
                QueryHostPort,
                "netcoreapp3.1",
                "csharp",
                FunctionsBindings.GetFunctionConfiguration(context));
        }

        [AfterScenario("useWorkflowEngineApi", "useWorkflowMessageProcessingApi", "useWorkflowQueryApi")]
        public static void WriteFunctionsOutput(FeatureContext featureContext)
        {
            FunctionsController functionsController = FunctionsBindings.GetFunctionsController(featureContext);
            functionsController.GetFunctionsOutput().WriteAllToConsoleAndClear();
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
