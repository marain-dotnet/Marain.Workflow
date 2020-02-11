// <copyright file="WorkflowFunctionBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.Specs.Bindings
{
    using System.Threading.Tasks;
    using Corvus.SpecFlow.Extensions;
    using TechTalk.SpecFlow;

    /// <summary>
    /// SpecFlow bindings to run the workflow engine function in-memory.
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
        public static string MessageProcessingHostBaseUrl = "http://localhost:" + MessageProcessingHostPort;
#pragma warning restore SA1401 // Fields should be private

        private const int EngineHostPort = 8765;
        private const int MessageProcessingHostPort = 8766;

        /// <summary>
        /// Sets up and runs the function using the functions runtime.
        /// </summary>
        /// <param name="context">The current <see cref="FeatureContext"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [BeforeFeature("@useWorkflowEngineApi", Order = ContainerBeforeFeatureOrder.ServiceProviderAvailable)]
        public static Task StartWorkflowEngineFunctionAsync(FeatureContext context)
        {
            return GetFunctionsController(context).StartFunctionsInstance(
                context,
                null,
                "Marain.Workflow.Api.EngineHost",
                EngineHostPort,
                "netstandard2.0");
        }

        /// <summary>
        /// Sets up and runs the function using the functions runtime.
        /// </summary>
        /// <param name="context">The current <see cref="FeatureContext"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [BeforeFeature("@useWorkflowMessageProcessingApi", Order = ContainerBeforeFeatureOrder.ServiceProviderAvailable)]
        public static Task StartWorkflowMessageProcessingFunctionAsync(FeatureContext context)
        {
            FunctionConfiguration config = GetFunctionConfiguration(context);
            config.EnvironmentVariables.Add("Workflow:EngineHostServiceBaseUrl", EngineHostBaseUrl);
            return GetFunctionsController(context).StartFunctionsInstance(
                context,
                null,
                "Marain.Workflow.Api.MessageProcessingHost",
                MessageProcessingHostPort,
                "netstandard2.0");
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

        private static FunctionsController GetFunctionsController(FeatureContext context)
        {
            if (!context.TryGetValue(out FunctionsController controller))
            {
                controller = new FunctionsController();
                context.Set(controller);
            }

            return controller;
        }

        private static FunctionConfiguration GetFunctionConfiguration(FeatureContext context)
        {
            if (!context.TryGetValue(out FunctionConfiguration configuration))
            {
                configuration = new FunctionConfiguration();
                context.Set(configuration);
            }

            return configuration;
        }
    }
}
