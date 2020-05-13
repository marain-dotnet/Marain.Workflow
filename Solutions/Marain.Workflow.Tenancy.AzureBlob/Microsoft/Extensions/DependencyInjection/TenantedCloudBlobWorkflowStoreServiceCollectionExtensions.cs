// <copyright file="TenantedCloudBlobWorkflowStoreServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using System.Linq;
    using Corvus.Azure.Storage.Tenancy;
    using Corvus.Extensions.Json;
    using Marain.Workflows;

    /// <summary>
    /// Service collection extensions to add the Cosmos implementation of workflow stores.
    /// </summary>
    public static class TenantedCloudBlobWorkflowStoreServiceCollectionExtensions
    {
        /// <summary>
        /// Gets the container defintion for the workflow instance change log.
        /// </summary>
        public static BlobStorageContainerDefinition WorkflowInstanceChangeLogContainerDefinition { get; } =
            new BlobStorageContainerDefinition("workflowinstancechangelog");

        /// <summary>
        /// Adds Azure blob storage-based implementation of <see cref="ITenantedWorkflowInstanceChangeLogFactory"/> to the service container.
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

            services.AddTenantCloudBlobContainerFactory(new TenantCloudBlobContainerFactoryOptions());
            services.AddSingleton<ITenantedWorkflowInstanceChangeLogFactory>(svc => new TenantedCloudBlobWorkflowInstanceChangeLogFactory(
                svc.GetRequiredService<IJsonSerializerSettingsProvider>(),
                svc.GetRequiredService<ITenantCloudBlobContainerFactory>(),
                WorkflowInstanceChangeLogContainerDefinition));

            return services;
        }
    }
}
