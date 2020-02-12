// <copyright file="WorkflowEngineServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using System.Linq;
    using Marain.Workflows;
    using Marain.Workflows.Api.Services;
    using Menes;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Extension methods for configuring DI for the the Workflow Engine OpenApi services.
    /// </summary>
    public static class WorkflowEngineServiceCollectionExtensions
    {
        /// <summary>
        /// Adds services required by workflow engine API.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">
        /// Configuration section to read root tenant default repository settings from.
        /// </param>
        /// <param name="configureHost">Optional callback for additional host configuration.</param>
        /// <returns>The service collection, to enable chaining.</returns>
        public static IServiceCollection AddTenantedWorkflowEngineApi(
            this IServiceCollection services,
            IConfiguration configuration,
            Action<IOpenApiHostConfiguration> configureHost = null)
        {
            // Verify that these services aren't already present
            Type engineServiceType = typeof(EngineService);

            // If any of the OpenApi services are already installed, assume we've already completed installation and return.
            if (services.Any(services => engineServiceType.IsAssignableFrom(services.ImplementationType)))
            {
                return services;
            }

            services.AddTenantedWorkflowEngine(configuration);

            services.AddOpenApiHttpRequestHosting<SimpleOpenApiContext>(config =>
            {
                config.Documents.RegisterOpenApiServiceWithEmbeddedDefinition<EngineService>();

                configureHost?.Invoke(config);

                config.Exceptions.Map<WorkflowNotFoundException>(404);
                config.Exceptions.Map<WorkflowInstanceNotFoundException>(404);
            });

            services.AddSingleton<IOpenApiService, EngineService>();

            return services;
        }

        /// <summary>
        /// Adds services required by to use the workflow engine.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">
        /// Configuration section to read root tenant default repository settings from.
        /// </param>
        /// <returns>The service collection, to enable chaining.</returns>
        public static IServiceCollection AddTenantedWorkflowEngine(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Verify that these services aren't already present
            Type engineFactoryServiceType = typeof(ITenantedWorkflowEngineFactory);

            // If any of the workflow are already installed, assume we've already completed installation and return.
            if (services.Any(services => engineFactoryServiceType.IsAssignableFrom(services.ImplementationType)))
            {
                return services;
            }

            services.AddLogging();

            services.AddTenantCloudBlobContainerFactory(configuration);
            services.AddTenantProviderBlobStore();

            services.AddTenantCosmosContainerFactory(configuration);
            services.AddTenantedWorkflowEngineFactory();
            services.AddTenantedAzureCosmosWorkflowStore(configuration);
            services.AddTenantedAzureCosmosWorkflowInstanceStore(configuration);

            services.RegisterCoreWorkflowContentTypes();

            services.AddAzureLeasing(c => c.ConnectionStringKey = "LeasingStorageAccountConnectionString");

            return services;
        }
    }
}