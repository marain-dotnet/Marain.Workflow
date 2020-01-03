// <copyright file="Host.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Functions.EngineHost
{
    using System.Threading.Tasks;
    using Marain.Identity.ManagedServiceIdentity.ClientAuthentication;
    using Marain.Logging;
    using Marain.OpenApi;
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
    /// The host for the functions app.
    /// </summary>
    public static class Host
    {
        /// <summary>
        /// The entry point for this functions app. Routes requests to
        /// <see cref="IOpenApiService"/> instances based on the service yaml file.
        /// </summary>
        /// <param name="req">
        /// The incoming request.
        /// </param>
        /// <param name="executionContext">
        /// The request context.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that completes when the request has been processed.
        /// </returns>
        [FunctionName("EngineHost-OpenApiHostRoot")]
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
            TelemetryOperationContext.Initialize(context.InvocationId.ToString(), context.FunctionName, "Endjin.Worflow.EndjinHost");
            Functions.InitializeContainer(context, (services, configuration) =>
                {
                    services.AddOpenApiHttpRequestHosting(config =>
                    {
                        config.Documents.RegisterOpenApiServiceWithEmbeddedDefinition<EngineService>();
                        config.Documents.AddSwaggerEndpoint();
                        config.Exceptions.Map<WorkflowNotFoundException>(404);
                        config.Exceptions.Map<WorkflowInstanceNotFoundException>(404);
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
                    services.SetRootTenantDefaultRepositoryConfiguration(configuration);
                    services.AddWorkflowEngineFactory();
                    services.RegisterCoreWorkflowContentTypes();
                    services.AddTenantKeyVaultOrConfigurationAccountKeyProvider();
                    services.AddAzureMsiBasedTokenSource();
                    services.AddAzureEventHubWorkflowTriggerQueue();
                    services.AddAzureLeasing(c => c.ConnectionStringKey = "LeasingStorageAccountConnectionString");
                    services.AddTelemetry();

                    services.AddSingleton<IOpenApiService, EngineService>();
                });
        }
    }
}