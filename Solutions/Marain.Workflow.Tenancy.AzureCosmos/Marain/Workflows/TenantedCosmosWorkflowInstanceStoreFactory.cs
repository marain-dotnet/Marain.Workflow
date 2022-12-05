// <copyright file="TenantedCosmosWorkflowInstanceStoreFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System.Threading.Tasks;

    using Corvus.CosmosClient;
    using Corvus.Storage.Azure.Cosmos.Tenancy;
    using Corvus.Tenancy;
    using Marain.Workflows.Storage;
    using Microsoft.Azure.Cosmos;

    using Newtonsoft.Json;

    /// <summary>
    /// Factory class for retrieving Cosmos-based instances of <see cref="IWorkflowStore"/> for specific <see cref="Tenant"/>s.
    /// </summary>
    public class TenantedCosmosWorkflowInstanceStoreFactory : ITenantedWorkflowInstanceStoreFactory
    {
        private readonly CosmosClientOptions clientOptions;

        private readonly ICosmosContainerSourceWithTenantLegacyTransition containerFactory;
        private readonly string logicalDatabaseName;
        private readonly string logicalInstanceContainerName;
        private readonly string instancePartitionKeyPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantedCosmosWorkflowInstanceStoreFactory"/> class.
        /// </summary>
        /// <param name="containerFactory">
        /// The <see cref="ICosmosContainerSourceWithTenantLegacyTransition"/> that will be used to
        /// create underlying <see cref="Container"/> instances for the content stores.
        /// </param>
        /// <param name="optionsFactory">
        /// Gets Cosmos Client options with suitable serialization configuration.
        /// </param>
        /// <param name="logicalDatabaseName">
        /// The logical Cosmos database name to use when creating or retrieving containers.
        /// </param>
        /// <param name="logicalInstanceContainerName">
        /// The logical name for the Cosmos container that holds workflow instances.
        /// </param>
        /// <param name="instancePartitionKeyPath">
        /// The partition key path for the workflow instance Cosmos container.
        /// </param>
        public TenantedCosmosWorkflowInstanceStoreFactory(
            ICosmosContainerSourceWithTenantLegacyTransition containerFactory,
            ICosmosOptionsFactory optionsFactory,
            string logicalDatabaseName,
            string logicalInstanceContainerName,
            string instancePartitionKeyPath)
        {
            this.containerFactory = containerFactory;
            this.logicalDatabaseName = logicalDatabaseName;
            this.logicalInstanceContainerName = logicalInstanceContainerName;
            this.instancePartitionKeyPath = instancePartitionKeyPath;

            this.clientOptions = optionsFactory.CreateCosmosClientOptions();
        }

        /// <inheritdoc/>
        public async Task<IWorkflowInstanceStore> GetWorkflowInstanceStoreForTenantAsync(ITenant tenant)
        {
            string v2Key = $"StorageConfiguration__{this.logicalDatabaseName}__{this.logicalInstanceContainerName}";
            Container container = await this.containerFactory.GetContainerForTenantAsync(
                tenant,
                v2Key,
                WorkflowCosmosTenancyPropertyKeys.Instances,
                containerName: this.logicalInstanceContainerName,
                partitionKeyPath: this.instancePartitionKeyPath,
                cosmosClientOptions: this.clientOptions).ConfigureAwait(false);

            // No need to cache these instances as they are lightweight wrappers around the container.
            return new CosmosWorkflowInstanceStore(container);
        }
    }
}