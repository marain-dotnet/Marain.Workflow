// <copyright file="TenantedCosmosWorkflowStoreServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using System.Linq;
    using Corvus.Azure.Cosmos.Tenancy;
    using Marain.Workflows;

    /// <summary>
    /// Service collection extensions to add the Cosmos implementation of workflow stores.
    /// </summary>
    public static class TenantedCosmosWorkflowStoreServiceCollectionExtensions
    {
        /// <summary>
        /// Gets the container definition that will be used for the tenanted workflow store.
        /// </summary>
        public static CosmosContainerDefinition WorkflowStoreContainerDefinition { get; } =
            new CosmosContainerDefinition("workflow", "definitions", "/id");

        /// <summary>
        /// Gets the container definition that will be used for the tenanted workflow instance store.
        /// </summary>
        public static CosmosContainerDefinition WorkflowInstanceStoreContainerDefinition { get; } =
            new CosmosContainerDefinition("workflow", "instances", "/id");

        /// <summary>
        /// Gets the container definition that will be used for the tenanted workflow instance store.
        /// </summary>
        public static CosmosContainerDefinition WorkflowInstanceChangeLogContainerDefinition { get; } =
            new CosmosContainerDefinition("workflow", "instancechangelog", "/workflowInstance/id");

        /// <summary>
        /// Adds Cosmos-based implementation of <see cref="ITenantedWorkflowStoreFactory"/> to the service container.
        /// </summary>
        /// <param name="services">The collection.</param>
        /// <returns>The configured <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddTenantedAzureCosmosWorkflowStore(
            this IServiceCollection services)
        {
            if (services.Any(s => s.ServiceType is ITenantedWorkflowStoreFactory))
            {
                return services;
            }

            services.AddTenantCosmosContainerFactory(new TenantCosmosContainerFactoryOptions());
            services.AddSingleton<ITenantedWorkflowStoreFactory>(svc => new TenantedCosmosWorkflowStoreFactory(
                svc.GetRequiredService<ITenantCosmosContainerFactory>(),
                WorkflowStoreContainerDefinition));

            return services;
        }

        /// <summary>
        /// Adds Cosmos-based implementation of <see cref="ITenantedWorkflowInstanceStoreFactory"/> to the service container.
        /// </summary>
        /// <param name="services">The collection.</param>
        /// <returns>The configured <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddTenantedAzureCosmosWorkflowInstanceStore(
            this IServiceCollection services)
        {
            if (services.Any(s => s.ServiceType is ITenantedWorkflowInstanceStoreFactory))
            {
                return services;
            }

            services.AddTenantCosmosContainerFactory(new TenantCosmosContainerFactoryOptions());
            services.AddSingleton<ITenantedWorkflowInstanceStoreFactory>(svc => new TenantedCosmosWorkflowInstanceStoreFactory(
                svc.GetRequiredService<ITenantCosmosContainerFactory>(),
                WorkflowInstanceStoreContainerDefinition));

            return services;
        }

        /// <summary>
        /// Adds Cosmos-based implementation of <see cref="ITenantedWorkflowInstanceChangeLogFactory"/> to the service container.
        /// </summary>
        /// <param name="services">The collection.</param>
        /// <returns>The configured <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddTenantedAzureCosmosWorkflowInstanceChangeLog(
            this IServiceCollection services)
        {
            if (services.Any(s => s.ServiceType is ITenantedWorkflowInstanceChangeLogFactory))
            {
                return services;
            }

            services.AddTenantCosmosContainerFactory(new TenantCosmosContainerFactoryOptions());
            services.AddSingleton<ITenantedWorkflowInstanceChangeLogFactory>(svc => new TenantedCosmosWorkflowInstanceChangeLogFactory(
                svc.GetRequiredService<ITenantCosmosContainerFactory>(),
                WorkflowInstanceChangeLogContainerDefinition));

            return services;
        }
    }
}
