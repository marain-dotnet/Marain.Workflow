// <copyright file="WorkflowEngineServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using System.Linq;

    using Corvus.Leasing;

    using Marain.Tenancy.Client;
    using Marain.Workflows;
    using Marain.Workflows.Api.Services;

    using Menes;

    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Extension methods for configuring DI for the Workflow Engine OpenApi services.
    /// </summary>
    public static class WorkflowEngineServiceCollectionExtensions
    {
        /// <summary>
        /// Adds services required by workflow engine API.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configureHost">Optional callback for additional host configuration.</param>
        /// <returns>The service collection, to enable chaining.</returns>
        [Obsolete("Use AddTenancyApiWithOpenApiActionResultHosting, or consider changing to AddTenancyApiWithAspNetPipelineHosting")]
        public static IServiceCollection AddTenantedWorkflowEngineApi(
            this IServiceCollection services,
            Action<IOpenApiHostConfiguration> configureHost = null)
        {
            // The new APIs require an IConfiguration to be passed directly instead of via the
            // callback mechanism that older versions of Functions forced us into. This method
            // Is still here only so we can generate a deprecation warning that tells people
            // what to use instead.
            throw new NotSupportedException();
        }

        /// <summary>
        /// Adds services required by workflow engine API.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">Configuration source.</param>
        /// <param name="configureHost">Optional callback for additional host configuration.</param>
        /// <returns>The service collection, to enable chaining.</returns>
        public static IServiceCollection AddTenantedWorkflowEngineApiWithAspNetPipelineHosting(
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
            services.AddOpenApiAspNetPipelineHosting<SimpleOpenApiContext>(MakeOpenApiHostConfigurer(configureHost));
            services.AddSingleton<IOpenApiService, EngineService>();

            return services;
        }

        /// <summary>
        /// Adds services required by workflow engine API.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">Configuration source.</param>
        /// <param name="configureHost">Optional callback for additional host configuration.</param>
        /// <returns>The service collection, to enable chaining.</returns>
        public static IServiceCollection AddTenantedWorkflowEngineApiWithOpenApiActionResultHosting(
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
            services.AddOpenApiActionResultHosting<SimpleOpenApiContext>(MakeOpenApiHostConfigurer(configureHost));
            services.AddSingleton<IOpenApiService, EngineService>();

            return services;
        }

        /// <summary>
        /// Adds services required by to use the workflow engine.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="configuration">Configuration source.</param>
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

            // Work around the fact that the tenancy client currently tries to fetch the root tenant on startup.
            services.AddMarainServiceConfiguration();

            services.AddMarainServicesTenancy();
            services.AddSingleton(configuration.GetSection("TenancyClient").Get<TenancyClientOptions>());
            services.AddTenantProviderServiceClient(true);

            services.AddServiceIdentityAzureTokenCredentialSourceFromLegacyConnectionString(configuration["AzureServicesAuthConnectionString"]);
            services.AddMicrosoftRestAdapterForServiceIdentityAccessTokenSource();

            // Workflow definitions get stored in blob storage
            services.AddTenantedBlobWorkflowStore();

            // Workflow instances get stored in CosmosDB
            services.AddTenantedAzureCosmosWorkflowInstanceStore();
            services.AddCosmosClientBuilderWithNewtonsoftJsonIntegration();

            services.AddTenantedWorkflowEngineFactory();

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

        /// <summary>
        /// API host configuration code common to all hosting modes.
        /// </summary>
        /// <param name="configureHost">Optional callback for additional host configuration.</param>
        /// <returns>
        /// A callback suitable for passing to <c>AddOpenApiActionResultHosting</c>.
        /// </returns>
        private static Action<IOpenApiHostConfiguration> MakeOpenApiHostConfigurer(Action<IOpenApiHostConfiguration> configureHost)
        {
            return (IOpenApiHostConfiguration config) =>
            {
                config.Documents.RegisterOpenApiServiceWithEmbeddedDefinition<EngineService>();

                configureHost?.Invoke(config);

                config.Exceptions.Map<WorkflowNotFoundException>(404);
                config.Exceptions.Map<WorkflowInstanceNotFoundException>(404);
                config.Exceptions.Map<WorkflowConflictException>(409);
                config.Exceptions.Map<WorkflowPreconditionFailedException>(412);
            };
        }
    }
}