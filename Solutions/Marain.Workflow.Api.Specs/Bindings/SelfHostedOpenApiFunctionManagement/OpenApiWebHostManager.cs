// <copyright file="OpenApiWebHostManager.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Corvus.SpecFlow.Extensions.SelfHostedOpenApiFunctionManagement
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Azure.WebJobs.Hosting;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Manages the execution of OpenApi services being run as part of a test scenario or feature.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class allows you to use the same Startup class that's used in an Azure function to run services in memory.
    /// To run the same services in memory as your function, first obtain an instance of this class using the static
    /// <see cref="GetInstance(SpecFlowContext)"/> method, supplying the current scenario or feature context. Then use the
    /// <see cref="StartFunctionAsync"/> method to add services, supplying the Startup class from your function and the
    /// base url (including port number where necessary) that the endpoints should be made available on.
    /// </para>
    /// <para>
    /// Note that this assumes you're using an instance method in your function host rather than the older static method
    /// approach. This means that your initialisation will have been moved into a Startup class that implements
    /// <c>IWebJobsStartup</c>; this is the class that should be supplied to <see cref="StartFunctionAsync"/>.
    /// </para>
    /// <para>
    /// You will normally make this call in a method tagged with either <c>BeforeScenario</c> or <c>BeforeFeature</c>. In the
    /// corresponding <c>AfterScenario</c> or <c>AfterFeature</c> method, you should call the
    /// <see cref="StopAllFunctionsAsync"/> method to stop and dispose the services. If you don't do this, the service will
    /// remain running. This may impact other features/scenarios as they will not be able to start a new instance of the
    /// service on the same Url.
    /// </para>
    /// </remarks>
    public class OpenApiWebHostManager
    {
        private const string ContextKey = "SelfHostedOpenApiFunctionManager_Instance";

        private readonly List<IWebHost> webHosts = new List<IWebHost>();

        /// <summary>
        /// Gets the current instance of the manager from the given context.
        /// </summary>
        /// <param name="context">The <see cref="SpecFlowContext"/> for the manager.</param>
        /// <returns>
        /// The instance of the <see cref="OpenApiWebHostManager"/> for the current feature/scenario.
        /// </returns>
        public static OpenApiWebHostManager GetInstance(SpecFlowContext context)
        {
            if (!context.TryGetValue(ContextKey, out OpenApiWebHostManager manager))
            {
                manager = new OpenApiWebHostManager();
                context.Set(manager, ContextKey);
            }

            return manager;
        }

        /// <summary>
        /// Starts a new function host using the given Uri and startup class.
        /// </summary>
        /// <typeparam name="TFunctionStartup">The type of the startup class. This should be the type of the class from the
        /// function host project that is used to initialise the OpenApi services and dependencies.</typeparam>
        /// <param name="baseUrl">The url that the function will be exposed on.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task StartFunctionAsync<TFunctionStartup>(string baseUrl)
            where TFunctionStartup : IWebJobsStartup, new()
        {
            IWebHostBuilder builder = WebHost.CreateDefaultBuilder();
            builder.UseUrls(baseUrl);
            builder.UseStartup<OpenApiWebHostStartup<TFunctionStartup>>();
            IWebHost host = builder.Build();

            this.webHosts.Add(host);

            return host.StartAsync();
        }

        /// <summary>
        /// Stops all of the function hosts that were started via <see cref="StartFunctionAsync"/>.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task StopAllFunctionsAsync()
        {
            foreach (IWebHost current in this.webHosts)
            {
                await current.StopAsync().ConfigureAwait(false);
                current.Dispose();
            }
        }
    }
}
