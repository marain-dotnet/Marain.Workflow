// <copyright file="TenantedBlobWorkflowStoreServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using System.Linq;

    using Corvus.Extensions.Json;
    using Corvus.Storage.Azure.BlobStorage.Tenancy;

    using Marain.Workflows;

    /// <summary>
    /// Service collection extensions to add the Cosmos implementation of workflow stores.
    /// </summary>
    public static class TenantedBlobWorkflowStoreServiceCollectionExtensions
    {
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

            services.AddAzureBlobStorageClientSourceFromDynamicConfiguration();
            services.AddTenantBlobContainerFactory();
            services.AddBlobContainerV2ToV3Transition();
            services.AddSingleton<ITenantedWorkflowStoreFactory>(svc => new TenantedBlobWorkflowStoreFactory(
                svc.GetRequiredService<IBlobContainerSourceWithTenantLegacyTransition>(),
                WorkflowTenancyConstants.DefinitionsStoreTenantConfigKey,
                WorkflowTenancyConstants.DefinitionsStoreTenantConfigKeyV3,
                svc.GetRequiredService<IJsonSerializerSettingsProvider>()));

            return services;
        }
    }
}