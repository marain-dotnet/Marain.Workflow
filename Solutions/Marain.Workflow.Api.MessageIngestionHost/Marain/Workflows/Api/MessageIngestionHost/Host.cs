// <copyright file="Host.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.MessageIngestionHost
{
    using System.Threading.Tasks;
    using Menes;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;

    /// <summary>
    /// The host for the functions app.
    /// </summary>
    public class Host
    {
        private readonly IOpenApiHost<HttpRequest, IActionResult> host;

        /// <summary>
        /// Initializes a new instance of the <see cref="Host"/> class.
        /// </summary>
        /// <param name="host">The OpenApi host.</param>
        public Host(IOpenApiHost<HttpRequest, IActionResult> host)
        {
            this.host = host;
        }

        /// <summary>
        /// Azure Functions entry point.
        /// </summary>
        /// <param name="req">The <see cref="HttpRequest"/>.</param>
        /// <param name="executionContext">The context for the function execution.</param>
        /// <returns>An action result which comes from executing the function.</returns>
        [FunctionName("MessageIngestionHost-OpenApiHostRoot")]
        public Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "{*path}")]
            HttpRequest req,
            ExecutionContext executionContext)
        {
            return this.host.HandleRequestAsync(req.Path, req.Method, req, new { ExecutionContext = executionContext });
        }

        ////private static void Initialize(ExecutionContext context)
        ////{
        ////    TelemetryOperationContext.Initialize(context.InvocationId.ToString(), context.FunctionName, "Endjin.Worflow.MessageIngestionHost");
        ////    Functions.InitializeContainer(
        ////    context,
        ////    (services, configurationRoot) =>
        ////        {
        ////            services.AddOpenApiHttpRequestHosting(config =>
        ////            {
        ////                config.Documents.RegisterOpenApiServiceWithEmbeddedDefinition<MessageIngestionService>();
        ////                config.Documents.AddSwaggerEndpoint();
        ////            });

        ////            LoggerConfiguration loggerConfig = new LoggerConfiguration()
        ////                 .Enrich.FromLogContext()
        ////                 .MinimumLevel.Debug()
        ////                 .Enrich.WithProperty("InvocationId", context.InvocationId)
        ////                 .WriteTo.Logger(lc => lc.Filter.ByExcluding(Matching.FromSource("Endjin.OpenApi")).WriteTo.Console().MinimumLevel.Debug())
        ////                 .WriteTo.Logger(lc => lc.Filter.ByIncludingOnly(Matching.FromSource("Endjin.OpenApi")).WriteTo.Console().MinimumLevel.Debug())
        ////                 .Enrich.With<EventIdEnricher>();
        ////            Log.Logger = loggerConfig.CreateLogger();
        ////            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

        ////            services.AddEndjinJsonConverters();
        ////            services.AddWorkflowEngineFactory();
        ////            services.RegisterCoreWorkflowContentTypes();
        ////            services.AddAzureEventHubWorkflowTriggerQueue();
        ////            services.AddTelemetry(mod =>
        ////                {
        ////                    mod.IncludeDiagnosticSourceActivities.Add("Microsoft.Azure.EventHubs");
        ////                    mod.IncludeDiagnosticSourceActivities.Add("Microsoft.Azure.ServiceBus");
        ////                });
        ////            string operationsControlServiceBaseUrl = configurationRoot["Operations:ControlServiceBaseUrl"];
        ////            string operationsControlServiceResourceId = configurationRoot["Operations:ControlServiceAadResourceId"];
        ////            if (string.IsNullOrWhiteSpace(operationsControlServiceResourceId))
        ////            {
        ////                operationsControlServiceResourceId = null;
        ////            }

        ////            services.AddOperationsControlClient(new Uri(operationsControlServiceBaseUrl), operationsControlServiceResourceId);
        ////            services.AddAzureMsiBasedTokenSource();

        ////            // We can add all the services here
        ////            // We will only actually *provide* services that are in the YAML file(s) we load below
        ////            // So you can register everything, and use the yaml files you deploy to decide what is responded to by this instance
        ////            services.AddSingleton<IOpenApiService, MessageIngestionService>();
        ////        });
        ////}
    }
}