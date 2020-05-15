// <copyright file="TenantedCosmosWorkflowInstanceChangeLogFactory.cs" company="Endjin Limited">
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
    public class TenantedCosmosWorkflowInstanceChangeLogFactory : ITenantedWorkflowInstanceChangeLogFactory
    {
        private readonly ITenantCosmosContainerFactory containerFactory;
        private readonly CosmosContainerDefinition containerDefinition;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantedCosmosWorkflowInstanceChangeLogFactory"/> class.
        /// </summary>
        /// <param name="containerFactory">The <see cref="ITenantCosmosContainerFactory"/> that will be used to create
        /// underlying <see cref="Container"/> instances for the content stores.</param>
        /// <param name="containerDefinition">The <see cref="CosmosContainerDefinition"/> to use when creating tenanted
        /// <see cref="Container"/> instances.</param>
        public TenantedCosmosWorkflowInstanceChangeLogFactory(
            ITenantCosmosContainerFactory containerFactory,
            CosmosContainerDefinition containerDefinition)
        {
            this.containerFactory = containerFactory;
            this.containerDefinition = containerDefinition;
        }

        /// <inheritdoc/>
        public async Task<IWorkflowInstanceChangeLogReader> GetWorkflowInstanceChangeLogReaderForTenantAsync(ITenant tenant)
        {
            Container container = await Retriable.RetryAsync(() => this.containerFactory.GetContainerForTenantAsync(tenant, this.containerDefinition), CancellationToken.None, new Linear(TimeSpan.FromSeconds(20), 5), RetryOnCosmosRequestRateExceededPolicy.Instance).ConfigureAwait(false);

            // No need to cache these instances as they are lightweight wrappers around the container.
            return new CosmosWorkflowInstanceChangeLog(container);
        }

        /// <inheritdoc/>
        public async Task<IWorkflowInstanceChangeLogWriter> GetWorkflowInstanceChangeLogWriterForTenantAsync(ITenant tenant)
        {
            Container container = await Retriable.RetryAsync(() => this.containerFactory.GetContainerForTenantAsync(tenant, this.containerDefinition), CancellationToken.None, new Linear(TimeSpan.FromSeconds(20), 5), RetryOnCosmosRequestRateExceededPolicy.Instance).ConfigureAwait(false);

            // No need to cache these instances as they are lightweight wrappers around the container.
            return new CosmosWorkflowInstanceChangeLog(container);
        }
    }
}
