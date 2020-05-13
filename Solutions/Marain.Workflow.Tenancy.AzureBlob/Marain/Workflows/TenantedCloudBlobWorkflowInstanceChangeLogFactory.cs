// <copyright file="TenantedCloudBlobWorkflowInstanceChangeLogFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System.Threading.Tasks;
    using Corvus.Azure.Storage.Tenancy;
    using Corvus.Extensions.Json;
    using Corvus.Tenancy;
    using Marain.Workflows.Storage;
    using Microsoft.Azure.Storage.Blob;

    /// <summary>
    /// Factory class for retrieving Cosmos-based instances of <see cref="IWorkflowStore"/> for specific <see cref="Tenant"/>s.
    /// </summary>
    public class TenantedCloudBlobWorkflowInstanceChangeLogFactory : ITenantedWorkflowInstanceChangeLogFactory
    {
        private readonly ITenantCloudBlobContainerFactory containerFactory;
        private readonly BlobStorageContainerDefinition containerDefinition;
        private readonly IJsonSerializerSettingsProvider serializerSettingsProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantedCloudBlobWorkflowInstanceChangeLogFactory"/> class.
        /// </summary>
        /// <param name="serializerSettingsProvider">The JSON serializer settings provider.</param>
        /// <param name="containerFactory">The <see cref="ITenantCloudBlobContainerFactory"/> that will be used to create
        /// underlying <see cref="CloudBlobContainer"/> instances for the content stores.</param>
        /// <param name="containerDefinition">The <see cref="BlobStorageContainerDefinition"/> to use when creating tenanted
        /// <see cref="CloudBlobContainer"/> instances.</param>
        public TenantedCloudBlobWorkflowInstanceChangeLogFactory(
            IJsonSerializerSettingsProvider serializerSettingsProvider,
            ITenantCloudBlobContainerFactory containerFactory,
            BlobStorageContainerDefinition containerDefinition)
        {
            this.containerFactory = containerFactory;
            this.containerDefinition = containerDefinition;
            this.serializerSettingsProvider = serializerSettingsProvider;
        }

        /// <inheritdoc/>
        public async Task<IWorkflowInstanceChangeLogReader> GetWorkflowInstanceChangeLogReaderForTenantAsync(ITenant tenant)
        {
            CloudBlobContainer container = await this.containerFactory.GetBlobContainerForTenantAsync(tenant, this.containerDefinition).ConfigureAwait(false);

            // No need to cache these instances as they are lightweight wrappers around the container.
            return new CloudBlobWorkflowInstanceChangeLog(this.serializerSettingsProvider, container);
        }

        /// <inheritdoc/>
        public async Task<IWorkflowInstanceChangeLogWriter> GetWorkflowInstanceChangeLogWriterForTenantAsync(ITenant tenant)
        {
            CloudBlobContainer container = await this.containerFactory.GetBlobContainerForTenantAsync(tenant, this.containerDefinition).ConfigureAwait(false);

            // No need to cache these instances as they are lightweight wrappers around the container.
            return new CloudBlobWorkflowInstanceChangeLog(this.serializerSettingsProvider, container);
        }
    }
}
