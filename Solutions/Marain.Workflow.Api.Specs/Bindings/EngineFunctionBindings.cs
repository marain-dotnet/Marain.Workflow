// <copyright file="EngineFunctionBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.Specs.Bindings
{
    using System.Threading.Tasks;
    using Corvus.SpecFlow.Extensions;
    using Corvus.SpecFlow.Extensions.SelfHostedOpenApiFunctionManagement;
    using Marain.Workflows.Api.EngineHost;
    using TechTalk.SpecFlow;

    /// <summary>
    /// SpecFlow bindings to run the workflow engine function in-memory.
    /// </summary>
    [Binding]
    public static class EngineFunctionBindings
    {
        /// <summary>
        /// The base Url that can be used to access the in-memory API.
        /// </summary>
        public const string BaseUrl = "http://localhost:8765";

        /// <summary>
        /// Sets up and runs the function, using the <see cref="Startup"/> class to initialise the service provider.
        /// </summary>
        /// <param name="context">The current <see cref="FeatureContext"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [BeforeFeature("@useWorkflowEngineApi", Order = ContainerBeforeFeatureOrder.ServiceProviderAvailable)]
        public static async Task StartWorkflowEngineFunction(FeatureContext context)
        {
            await OpenApiWebHostManager.GetInstance(context).StartFunctionAsync<Startup>(BaseUrl).ConfigureAwait(false);
        }

        /// <summary>
        /// Tears down the in-memory service.
        /// </summary>
        /// <param name="context">The current <see cref="FeatureContext"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [AfterFeature("@useWorkflowEngineApi")]
        public static Task StopWorkflowEngineFunction(FeatureContext context)
        {
            return context.RunAndStoreExceptionsAsync(async () => await OpenApiWebHostManager.GetInstance(context).StopAllFunctionsAsync().ConfigureAwait(false));
        }
    }
}
