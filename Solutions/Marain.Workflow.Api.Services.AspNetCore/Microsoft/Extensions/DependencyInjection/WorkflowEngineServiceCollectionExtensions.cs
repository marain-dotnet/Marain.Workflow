// <copyright file="WorkflowEngineServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using System.Linq;
    using Corvus.Azure.Cosmos.Tenancy;
    using Corvus.Leasing;
    using Marain.Tenancy.Client;
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
        /// <param name="configureHost">Optional callback for additional host configuration.</param>
        /// <returns>The service collection, to enable chaining.</returns>
        public static IServiceCollection AddTenantedWorkflowEngineApi(
            this IServiceCollection services,
            Action<IOpenApiHostConfiguration> configureHost = null)
        {
            // Verify that these services aren't already present
            Type engineServiceType = typeof(EngineService);

            // If any of the OpenApi services are already installed, assume we've already completed installation and return.
            if (services.Any(services => engineServiceType.IsAssignableFrom(services.ImplementationType)))
            {
                return services;
            }

            services.AddTenantedWorkflowEngine();

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
        /// <returns>The service collection, to enable chaining.</returns>
        public static IServiceCollection AddTenantedWorkflowEngine(
            this IServiceCollection services)
        {
            // Verify that these services aren't already present
            Type engineFactoryServiceType = typeof(ITenantedWorkflowEngineFactory);

            // If any of the workflow are already installed, assume we've already completed installation and return.
            if (services.Any(services => engineFactoryServiceType.IsAssignableFrom(services.ImplementationType)))
            {
                return services;
            }

            services.AddLogging();

            // Work around the fact that the tenancy client currently tries to fetch the root tenant on startup.
            services.AddMarainServiceConfiguration();

            services.AddRootTenant();
            services.AddMarainServicesTenancy();
            services.AddSingleton(sp => sp.GetRequiredService<IConfiguration>().GetSection("TenancyClient").Get<TenancyClientOptions>());
            services.AddTenantProviderServiceClient();

            services.AddAzureManagedIdentityBasedTokenSource();

            services.AddTenantCosmosContainerFactory(sp =>
            {
                IConfiguration config = sp.GetRequiredService<IConfiguration>();

                return config.GetSection("TenantCosmosContainerFactoryOptions").Get<TenantCosmosContainerFactoryOptions>()
                    ?? new TenantCosmosContainerFactoryOptions();
            });

            services.AddTenantedWorkflowEngineFactory();
            services.AddTenantedAzureCosmosWorkflowStore();
            services.AddTenantedAzureCosmosWorkflowInstanceStore();

            services.RegisterCoreWorkflowContentTypes();

            services.AddAzureLeasing(svc =>
            {
                IConfiguration config = svc.GetRequiredService<IConfiguration>();
                return new AzureLeaseProviderOptions
                {
                    StorageAccountConnectionString = config["LeasingStorageAccountConnectionString"],
                };
            });

            return services;
        }
    }
}