// <copyright file="Startup.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

[assembly: Microsoft.Azure.WebJobs.Hosting.WebJobsStartup(typeof(Marain.Workflows.Api.MessagePreProcessingHost.Shared.Startup))]

namespace Marain.Workflows.Api.MessagePreProcessingHost.Shared
{
    using System;
    using System.Linq;
    using Marain.Operations.Client.OperationsControl;
    using Marain.Workflow.Api.EngineHost.Client;
    using Marain.Workflows.Api.MessagePreProcessingHost.OpenApi;
    using Menes;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Startup code for the Function.
    /// </summary>
    public class Startup : IWebJobsStartup
    {
        /// <inheritdoc/>
        public void Configure(IWebJobsBuilder builder)
        {
            IServiceCollection services = builder.Services;

            IConfigurationRoot root = Configure(services);
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Debug);
            });

            var uri = new Uri(root["Operations:ControlServiceBaseUrl"]);
            services.AddOperationsControlClient(uri);

            var workflowEngineClientConfig = new WorkflowEngineClientConfiguration
            {
                BaseUrl = root["Workflow:EngineHostServiceBaseUrl"],
            };

            services.AddTenantedWorkflowEngineClient(workflowEngineClientConfig);

            services.AddTenantedWorkflowEngine(root);
            AddMessageProcessingMenesServices(services, root);
        }

        /// <summary>
        /// Adds services required by workflow engine API.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">
        /// Configuration section to read root tenant default repository settings from.
        /// </param>
        /// <returns>The service collection, to enable chaining.</returns>
        private static IServiceCollection AddMessageProcessingMenesServices(
            IServiceCollection services,
            IConfiguration configuration)
        {
            // Verify that these services aren't already present
            Type ingestionServiceType = typeof(MessageIngestionService);

            // If any of the OpenApi services are already installed, assume we've already completed installation and return.
            if (services.Any(services => ingestionServiceType.IsAssignableFrom(services.ImplementationType)))
            {
                return services;
            }

            services.AddTenantedWorkflowEngine(configuration);

            services.AddOpenApiHttpRequestHosting<DurableFunctionsOpenApiContext>(config =>
            {
                config.Documents.RegisterOpenApiServiceWithEmbeddedDefinition<MessageIngestionService>();

                config.Documents.AddSwaggerEndpoint();

                config.Exceptions.Map<WorkflowNotFoundException>(404);
                config.Exceptions.Map<WorkflowInstanceNotFoundException>(404);
            });

            services.AddSingleton<IOpenApiService, MessageIngestionService>();

            return services;
        }

        private static IConfigurationRoot Configure(IServiceCollection services)
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot root = configurationBuilder.Build();
            services.AddSingleton(root);
            return root;
        }
    }
}