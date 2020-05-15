// <copyright file="TenantedCosmosWorkflowStoreFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Corvus.Azure.Cosmos.Tenancy;
    using Corvus.Extensions.Cosmos;
    using Corvus.Retry;
    using Corvus.Retry.Strategies;
    using Corvus.Tenancy;
    using Marain.Workflows.Internal;
    using Marain.Workflows.Storage;
    using Microsoft.Azure.Cosmos;

    /// <summary>
    /// Factory class for retrieving Cosmos-based instances of <see cref="IWorkflowStore"/> for specific <see cref="Tenant"/>s.
    /// </summary>
    public class TenantedCosmosWorkflowStoreFactory : ITenantedWorkflowStoreFactory
    {
        private readonly ITenantCosmosContainerFactory containerFactory;
        private readonly CosmosContainerDefinition containerDefinition;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantedCosmosWorkflowStoreFactory"/> class.
        /// </summary>
        /// <param name="containerFactory">The <see cref="ITenantCosmosContainerFactory"/> that will be used to create
        /// underlying <see cref="Container"/> instances for the content stores.</param>
        /// <param name="containerDefinition">The <see cref="CosmosContainerDefinition"/> to use when creating tenanted
        /// <see cref="Container"/> instances.</param>
        public TenantedCosmosWorkflowStoreFactory(
            ITenantCosmosContainerFactory containerFactory,
            CosmosContainerDefinition containerDefinition)
        {
            this.containerFactory = containerFactory;
            this.containerDefinition = containerDefinition;
        }

        /// <inheritdoc/>
        public async Task<IWorkflowStore> GetWorkflowStoreForTenantAsync(ITenant tenant)
        {
            Container container = await WorkflowRetryHelper.ExecuteWithRetryRulesAsync(() => this.containerFactory.GetContainerForTenantAsync(tenant, this.containerDefinition)).ConfigureAwait(false);

            // No need to cache these instances as they are lightweight wrappers around the container.
            return new CosmosWorkflowStore(container);
        }
    }
}
