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

            var containerDefinition = new CosmosContainerDefinition("workflow", "workflows", "/id");

            services.AddTenantCosmosContainerFactory(new TenantCosmosContainerFactoryOptions());
            services.AddSingleton<ITenantedWorkflowStoreFactory>(svc => new TenantedCosmosWorkflowStoreFactory(
                svc.GetRequiredService<ITenantCosmosContainerFactory>(),
                containerDefinition));

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

            var containerDefinition = new CosmosContainerDefinition("workflow", "workflowinstances", "/id");

            services.AddTenantCosmosContainerFactory(new TenantCosmosContainerFactoryOptions());
            services.AddSingleton<ITenantedWorkflowInstanceStoreFactory>(svc => new TenantedCosmosWorkflowInstanceStoreFactory(
                svc.GetRequiredService<ITenantCosmosContainerFactory>(),
                containerDefinition));

            return services;
        }
    }
}
