// <copyright file="Startup.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

[assembly: Microsoft.Azure.WebJobs.Hosting.WebJobsStartup(typeof(Marain.Workflows.Api.MessageProcessingHost.Shared.Startup))]

namespace Marain.Workflows.Api.MessageProcessingHost.Shared
{
    using System;
    using System.Linq;
    using Marain.Operations.Client.OperationsControl;
    using Marain.Workflows.Api.MessageProcessingHost.OpenApi;
    using Marain.Workflows.EngineHost.Client;
    using Menes;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Startup code for the Function.
    /// </summary>
    public class Startup : IWebJobsStartup
    {
        /// <inheritdoc/>
        public void Configure(IWebJobsBuilder builder)
        {
            IServiceCollection services = builder.Services;

            services.AddApplicationInsightsInstrumentationTelemetry();
            services.AddLogging();

            services.AddOperationsControlClient(
                sp =>
                {
                    IConfiguration config = sp.GetRequiredService<IConfiguration>();

                    string baseUrl = config["Operations:ControlServiceBaseUrl"];

                    if (string.IsNullOrEmpty(baseUrl))
                    {
                        throw new InvalidOperationException("Cannot find a configuration value for 'Operations:ControlServiceBaseUrl'. Please add this configuration value and retry.");
                    }

                    return new MarainOperationsControlClientOptions
                    {
                        OperationsControlServiceBaseUri = new Uri(baseUrl),
                        ResourceIdForMsiAuthentication = config["Operations:ResourceIdForMsiAuthentication"],
                    };
                });

            services.AddMarainWorkflowEngineClient(sp =>
            {
                IConfiguration config = sp.GetRequiredService<IConfiguration>();

                return config.GetSection("Workflow:EngineClient").Get<MarainWorkflowEngineClientOptions>();
            });

            services.AddTenantedWorkflowEngine();
            AddMessageProcessingMenesServices(services);
        }

        /// <summary>
        /// Adds services required by workflow engine API.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The service collection, to enable chaining.</returns>
        private static IServiceCollection AddMessageProcessingMenesServices(
            IServiceCollection services)
        {
            // Verify that these services aren't already present
            Type ingestionServiceType = typeof(MessageIngestionService);

            // If any of the OpenApi services are already installed, assume we've already completed installation and return.
            if (services.Any(services => ingestionServiceType.IsAssignableFrom(services.ImplementationType)))
            {
                return services;
            }

            services.AddTenantedWorkflowEngine();

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
    }
}