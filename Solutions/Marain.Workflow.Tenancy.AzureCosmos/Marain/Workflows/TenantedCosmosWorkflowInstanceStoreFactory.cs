﻿// <copyright file="TenantedCosmosWorkflowInstanceStoreFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System.Threading.Tasks;
    using Corvus.Azure.Cosmos.Tenancy;
    using Corvus.Tenancy;
    using Marain.Workflows.Storage;
    using Microsoft.Azure.Cosmos;

    /// <summary>
    /// Factory class for retrieving Cosmos-based instances of <see cref="IWorkflowStore"/> for specific <see cref="Tenant"/>s.
    /// </summary>
    public class TenantedCosmosWorkflowInstanceStoreFactory : ITenantedWorkflowInstanceStoreFactory
    {
        private readonly ITenantCosmosContainerFactory containerFactory;
        private readonly CosmosContainerDefinition containerDefinition;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantedCosmosWorkflowInstanceStoreFactory"/> class.
        /// </summary>
        /// <param name="containerFactory">The <see cref="ITenantCosmosContainerFactory"/> that will be used to create
        /// underlying <see cref="Container"/> instances for the content stores.</param>
        /// <param name="containerDefinition">The <see cref="CosmosContainerDefinition"/> to use when creating tenanted
        /// <see cref="Container"/> instances.</param>
        public TenantedCosmosWorkflowInstanceStoreFactory(
            ITenantCosmosContainerFactory containerFactory,
            CosmosContainerDefinition containerDefinition)
        {
            this.containerFactory = containerFactory;
            this.containerDefinition = containerDefinition;
        }

        /// <inheritdoc/>
        public async Task<IWorkflowInstanceStore> GetWorkflowInstanceStoreForTenantAsync(ITenant tenant)
        {
            Container container = await this.containerFactory.GetContainerForTenantAsync(tenant, this.containerDefinition).ConfigureAwait(false);

            // No need to cache these instances as they are lightweight wrappers around the container.
            return new CosmosWorkflowInstanceStore(container);
        }
    }
}
