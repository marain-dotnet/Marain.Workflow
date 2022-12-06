// <copyright file="TenantedCosmosWorkflowStoreFactory.cs" company="Endjin Limited">
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
    public class TenantedCosmosWorkflowStoreFactory : ITenantedWorkflowStoreFactory
    {
        private readonly CosmosClientOptions clientOptions;

        private readonly ICosmosContainerSourceWithTenantLegacyTransition containerFactory;
        private readonly string logicalDatabaseName;
        private readonly string logicalDefinitionContainerName;
        private readonly string definitionPartitionKeyPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantedCosmosWorkflowStoreFactory"/> class.
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
        /// <param name="logicalDefinitionContainerName">
        /// The logical name for the Cosmos container that holds workflow definitions.
        /// </param>
        /// <param name="definitionPartitionKeyPath">
        /// The partition key path for the workflow definition Cosmos container.
        /// </param>
        public TenantedCosmosWorkflowStoreFactory(
            ICosmosContainerSourceWithTenantLegacyTransition containerFactory,
            ICosmosOptionsFactory optionsFactory,
            string logicalDatabaseName,
            string logicalDefinitionContainerName,
            string definitionPartitionKeyPath)
        {
            this.containerFactory = containerFactory;
            this.logicalDatabaseName = logicalDatabaseName;
            this.logicalDefinitionContainerName = logicalDefinitionContainerName;
            this.definitionPartitionKeyPath = definitionPartitionKeyPath;

            this.clientOptions = optionsFactory.CreateCosmosClientOptions();
        }

        /// <inheritdoc/>
        public async Task<IWorkflowStore> GetWorkflowStoreForTenantAsync(ITenant tenant)
        {
            string v2Key = $"StorageConfiguration__{this.logicalDatabaseName}__{this.logicalDefinitionContainerName}";
            Container container = await this.containerFactory.GetContainerForTenantAsync(
                tenant,
                v2Key,
                WorkflowCosmosTenancyPropertyKeys.Definitions,
                this.logicalDatabaseName,
                this.logicalDefinitionContainerName,
                this.definitionPartitionKeyPath,
                cosmosClientOptions: this.clientOptions).ConfigureAwait(false);

            // No need to cache these instances as they are lightweight wrappers around the container.
            return new CosmosWorkflowStore(container);
        }
    }
}