// <copyright file="HighTrafficIngestionHostFunctionBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.Specs.Bindings
{
    using System.Threading.Tasks;
    using Corvus.SpecFlow.Extensions;
    using Corvus.SpecFlow.Extensions.SelfHostedOpenApiFunctionManagement;
    using TechTalk.SpecFlow;

    /// <summary>
    /// SpecFlow bindings for hosting the high traffic ingestion host in-memory.
    /// </summary>
    [Binding]
    public static class HighTrafficIngestionHostFunctionBindings
    {
        /// <summary>
        /// The base Url that can be used to access the in-memory API.
        /// </summary>
        public const string BaseUrl = "http://localhost:8764";

        /// <summary>
        /// Sets up and runs the function, using the <see cref="Api.MessageIngestionHost.Startup"/> class to initialise the service provider.
        /// </summary>
        /// <param name="context">The current <see cref="FeatureContext"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [BeforeFeature("@useHighTrafficIngestionApi", Order = ContainerBeforeFeatureOrder.ServiceProviderAvailable)]
        public static async Task StartHighTrafficIngestionFunction(FeatureContext context)
        {
            await OpenApiWebHostManager.GetInstance(context).StartFunctionAsync<Api.MessageIngestionHost.Startup>(BaseUrl).ConfigureAwait(false);
        }

        /// <summary>
        /// Tears down the in-memory service.
        /// </summary>
        /// <param name="context">The current <see cref="FeatureContext"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [AfterFeature("@useHighTrafficIngestionApi")]
        public static Task StopHighTrafficIngestionFunction(FeatureContext context)
        {
            return context.RunAndStoreExceptionsAsync(async () => await OpenApiWebHostManager.GetInstance(context).StopAllFunctionsAsync().ConfigureAwait(false));
        }
    }
}
