// <copyright file="TenantedCosmosWorkflowStoreServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using System.Linq;

    using Corvus.CosmosClient;
    using Corvus.Storage.Azure.Cosmos.Tenancy;

    using Marain.Workflows;

    /// <summary>
    /// Service collection extensions to add the Cosmos implementation of workflow stores.
    /// </summary>
    public static class TenantedCosmosWorkflowStoreServiceCollectionExtensions
    {
        /// <summary>
        /// The logical name for the Cosmos Database containing the workflow definition store and
        /// the workflow instance store.
        /// </summary>
        /// <remarks>
        /// This is used as the basis for generating tenant-specific database names.
        /// </remarks>
        public const string WorkflowStoreLogicalDatabaseName = "workflow";

        /// <summary>
        /// The logical name for the workflow definition Cosmos container.
        /// </summary>
        /// <remarks>
        /// This might also be the real name. But in scenarios where multiple tenants share a
        /// single Cosmos database (e.g., to enable database-level throughput provisioning to
        /// reduce costs for tenants that don't need privately provisioned throughput) the
        /// real name will be tenant-specific.
        /// </remarks>
        public const string WorkflowDefinitionStoreLogicalContainerName = "definitions";

        /// <summary>
        /// The partition key path for the workflow definition Cosmos container.
        /// </summary>
        public const string WorkflowDefinitionStorePartitionKeyPath = "/id";

        /// <summary>
        /// The logical name for the workflow instance Cosmos container.
        /// </summary>
        /// <remarks>
        /// This might also be the real name. But in scenarios where multiple tenants share a
        /// single Cosmos database (e.g., to enable database-level throughput provisioning to
        /// reduce costs for tenants that don't need privately provisioned throughput) the
        /// real name will be tenant-specific.
        /// </remarks>
        public const string WorkflowInstanceStoreLogicalContainerName = "instances";

        /// <summary>
        /// The partition key path for the workflow instance Cosmos container.
        /// </summary>
        public const string WorkflowInstanceStorePartitionKeyPath = "/id";

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

            services.AddTenantCosmosContainerFactory();
            services.AddCosmosContainerV2ToV3Transition();
            services.AddSingleton<ITenantedWorkflowStoreFactory>(svc => new TenantedCosmosWorkflowStoreFactory(
                svc.GetRequiredService<ICosmosContainerSourceWithTenantLegacyTransition>(),
                svc.GetRequiredService<ICosmosOptionsFactory>(),
                WorkflowStoreLogicalDatabaseName,
                WorkflowDefinitionStoreLogicalContainerName,
                WorkflowDefinitionStorePartitionKeyPath));

            return services;
        }

        /// <summary>
        /// Adds Cosmos-based implementation of <see cref="ITenantedWorkflowStoreFactory"/> to the service container.
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

            services.AddTenantCosmosContainerFactory();
            services.AddCosmosContainerV2ToV3Transition();
            services.AddSingleton<ITenantedWorkflowInstanceStoreFactory>(svc => new TenantedCosmosWorkflowInstanceStoreFactory(
                svc.GetRequiredService<ICosmosContainerSourceWithTenantLegacyTransition>(),
                svc.GetRequiredService<ICosmosOptionsFactory>(),
                WorkflowStoreLogicalDatabaseName,
                WorkflowInstanceStoreLogicalContainerName,
                WorkflowInstanceStorePartitionKeyPath));

            return services;
        }
    }
}