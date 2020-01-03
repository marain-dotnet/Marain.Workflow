// <copyright file="Host.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Functions.MessageIngestionHost
{
    using System;
    using System.Threading.Tasks;
    using Marain.Identity.ManagedServiceIdentity.ClientAuthentication;
    using Marain.Logging;
    using Marain.OpenApi;
    using Marain.Operations.Client.OperationsControl;
    using Marain.Telemetry;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Serilog;
    using Serilog.Filters;
    using Functions = Marain.Functions;

    /// <summary>
    ///     The host for the functions app.
    /// </summary>
    public static class Host
    {
        /// <summary>
        ///     The entry point for this functions app. Routes requests to
        ///     <see cref="IOpenApiService" /> instances based on the service yaml file.
        /// </summary>
        /// <param name="req">
        ///     The incoming request.
        /// </param>
        /// <param name="executionContext">
        ///     The request context.
        /// </param>
        /// <returns>
        ///     A <see cref="Task" /> that completes when the request has been processed.
        /// </returns>
        [FunctionName("MessageIngestionHost-OpenApiHostRoot")]
        public static Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "{*path}")]
            HttpRequest req,
            ExecutionContext executionContext)
        {
            Initialize(executionContext);

            return req.HandleRequestAsync();
        }

        private static void Initialize(ExecutionContext context)
        {
            TelemetryOperationContext.Initialize(context.InvocationId.ToString(), context.FunctionName, "Endjin.Worflow.MessageIngestionHost");
            Functions.InitializeContainer(
                context,
                (services, configurationRoot) =>
                    {
                        services.AddOpenApiHttpRequestHosting(config =>
                        {
                            config.Documents.RegisterOpenApiServiceWithEmbeddedDefinition<MessageIngestionService>();
                            config.Documents.AddSwaggerEndpoint();
                        });

                        LoggerConfiguration loggerConfig = new LoggerConfiguration()
                             .Enrich.FromLogContext()
                             .MinimumLevel.Debug()
                             .Enrich.WithProperty("InvocationId", context.InvocationId)
                             .WriteTo.Logger(lc => lc.Filter.ByExcluding(Matching.FromSource("Endjin.OpenApi")).WriteTo.Console().MinimumLevel.Debug())
                             .WriteTo.Logger(lc => lc.Filter.ByIncludingOnly(Matching.FromSource("Endjin.OpenApi")).WriteTo.Console().MinimumLevel.Debug())
                             .Enrich.With<EventIdEnricher>();
                        Log.Logger = loggerConfig.CreateLogger();
                        services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

                        services.AddEndjinJsonConverters();
                        services.AddWorkflowEngineFactory();
                        services.RegisterCoreWorkflowContentTypes();
                        services.AddAzureEventHubWorkflowTriggerQueue();
                        services.AddTelemetry(mod =>
                            {
                                mod.IncludeDiagnosticSourceActivities.Add("Microsoft.Azure.EventHubs");
                                mod.IncludeDiagnosticSourceActivities.Add("Microsoft.Azure.ServiceBus");
                            });
                        string operationsControlServiceBaseUrl = configurationRoot["Operations:ControlServiceBaseUrl"];
                        string operationsControlServiceResourceId = configurationRoot["Operations:ControlServiceAadResourceId"];
                        if (string.IsNullOrWhiteSpace(operationsControlServiceResourceId))
                        {
                            operationsControlServiceResourceId = null;
                        }

                        services.AddOperationsControlClient(new Uri(operationsControlServiceBaseUrl), operationsControlServiceResourceId);
                        services.AddAzureMsiBasedTokenSource();

                        // We can add all the services here
                        // We will only actually *provide* services that are in the YAML file(s) we load below
                        // So you can register everything, and use the yaml files you deploy to decide what is responded to by this instance
                        services.AddSingleton<IOpenApiService, MessageIngestionService>();
                    });
        }
    }
}