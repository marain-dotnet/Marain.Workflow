// <copyright file="TenantedBlobWorkflowStoreServiceCollectionExtensions.cs" company="Endjin Limited">
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
    public static class TenantedBlobWorkflowStoreServiceCollectionExtensions
    {
        /// <summary>
        /// Gets the container definition that will be used for the tenanted workflow store.
        /// </summary>
        public static BlobStorageContainerDefinition WorkflowStoreContainerDefinition { get; } =
            new BlobStorageContainerDefinition("workflowdefinitions");

        /// <summary>
        /// Adds Cosmos-based implementation of <see cref="ITenantedWorkflowStoreFactory"/> to the service container.
        /// </summary>
        /// <param name="services">The collection.</param>
        /// <returns>The configured <see cref="IServiceCollection"/>.</returns>
        public static IServiceCollection AddTenantedBlobWorkflowStore(
            this IServiceCollection services)
        {
            if (services.Any(s => s.ServiceType is ITenantedWorkflowStoreFactory))
            {
                return services;
            }

            services.AddTenantCloudBlobContainerFactory(new TenantCloudBlobContainerFactoryOptions());
            services.AddSingleton<ITenantedWorkflowStoreFactory>(svc => new TenantedBlobWorkflowStoreFactory(
                svc.GetRequiredService<ITenantCloudBlobContainerFactory>(),
                WorkflowStoreContainerDefinition,
                svc.GetRequiredService<IJsonSerializerSettingsProvider>()));

            return services;
        }
    }
}