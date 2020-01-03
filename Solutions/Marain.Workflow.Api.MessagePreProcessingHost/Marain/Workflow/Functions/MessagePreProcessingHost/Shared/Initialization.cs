// <copyright file="Initialization.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflow.Functions.MessagePreProcessingHost.Shared
{
    using System;
    using Marain.Identity.ManagedServiceIdentity.ClientAuthentication;
    using Marain.Logging;
    using Marain.Operations.Client.OperationsControl;
    using Marain.Workflow.Functions.EngineHost;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Serilog;
    using Serilog.Filters;
    using Functions = Marain.Functions;

    /// <summary>
    ///     Helper class to contain function initialization code.
    /// </summary>
    public static class Initialization
    {
        /// <summary>
        ///     Initializes the container.
        /// </summary>
        /// <param name="context">
        ///     The execution context of the function.
        /// </param>
        public static void Initialize(ExecutionContext context)
        {
            Functions.InitializeContainer(
                context,
                (services, configurationRoot) =>
                    {
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

                        services.SetRootTenantDefaultRepositoryConfiguration(configurationRoot);
                        services.AddAzureLeasing(c => c.ConnectionStringKey = "LeasingStorageAccountConnectionString");
                        services.AddTenantKeyVaultOrConfigurationAccountKeyProvider();
                        services.RegisterCoreWorkflowContentTypes();
                        services.AddTelemetry();

                        string operationsControlServiceBaseUrl = configurationRoot["Operations:ControlServiceBaseUrl"];
                        string operationsControlServiceResourceId = configurationRoot["Operations:ControlServiceAadResourceId"];
                        if (string.IsNullOrWhiteSpace(operationsControlServiceResourceId))
                        {
                            operationsControlServiceResourceId = null;
                        }

                        services.AddOperationsControlClient(new Uri(operationsControlServiceBaseUrl), operationsControlServiceResourceId);

                        string workflowEngineServiceBaseUri = configurationRoot["Workflow:EngineHostServiceBaseUrl"];
                        string workflowEngineServiceResourceId = configurationRoot["Workflow:EngineHostServiceAadResourceId"];
                        if (string.IsNullOrWhiteSpace(workflowEngineServiceResourceId))
                        {
                            workflowEngineServiceResourceId = null;
                        }

                        services.AddEndjinWorkflowEngineClient(new Uri(workflowEngineServiceBaseUri), workflowEngineServiceResourceId);

                        services.AddAzureMsiBasedTokenSource();
                    });
        }
    }
}