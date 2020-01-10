using System.Threading.Tasks;
using Corvus.SpecFlow.Extensions;
using Corvus.SpecFlow.Extensions.SelfHostedOpenApiFunctionManagement;
using Marain.Workflows.Functions.EngineHost;
using TechTalk.SpecFlow;

namespace Marain.Workflows.Functions.Specs.Bindings
{
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
        /// <remarks>
        /// As part of the initialisation, an <see cref="HttpClient"/> will be created and stored in
        /// the <see cref="FeatureContext"/>.
        /// </remarks>
        [BeforeFeature("@useWorkflowEngineApi", Order = ContainerBeforeFeatureOrder.ServiceProviderAvailable)]
        public static async Task StartContentManagementFunction(FeatureContext context)
        {
            await OpenApiWebHostManager.GetInstance(context).StartFunctionAsync<Functions.EngineHost.Startup>(BaseUrl).ConfigureAwait(false);
        }

        /// <summary>
        /// Tears down the in-memory service.
        /// </summary>
        /// <param name="context">The current <see cref="FeatureContext"/>.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [AfterFeature("@useWorkflowEngineApi")]
        public static Task StopContentManagementFunction(FeatureContext context)
        {
            return context.RunAndStoreExceptionsAsync(async () =>
            {
                await OpenApiWebHostManager.GetInstance(context).StopAllFunctionsAsync().ConfigureAwait(false);
            });
        }
    }
}
