// <copyright file="TenantedBlobWorkflowStoreFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System;
    using System.Threading.Tasks;
    using Corvus.Azure.Storage.Tenancy;
    using Corvus.Extensions.Json;
    using Corvus.Tenancy;
    using Marain.Workflows.Storage;
    using Microsoft.Azure.Storage.Blob;

    /// <summary>
    /// Factory class for retrieving Azure blob storage-based instances of <see cref="IWorkflowStore"/> for specific
    /// <see cref="Tenant"/>s.
    /// </summary>
    public class TenantedBlobWorkflowStoreFactory : ITenantedWorkflowStoreFactory
    {
        private readonly ITenantCloudBlobContainerFactory containerFactory;
        private readonly BlobStorageContainerDefinition containerDefinition;
        private readonly IJsonSerializerSettingsProvider serializerSettingsProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantedBlobWorkflowStoreFactory"/> class.
        /// </summary>
        /// <param name="containerFactory">The <see cref="ITenantCloudBlobContainerFactory"/> that will be used to create
        /// underlying <see cref="CloudBlobContainer"/> instances for the content stores.</param>
        /// <param name="containerDefinition">The <see cref="BlobStorageContainerDefinition"/> to use when creating tenanted
        /// <see cref="CloudBlobContainer"/> instances.</param>
        /// <param name="serializerSettingsProvider">The current <see cref="IJsonSerializerSettingsProvider"/>.</param>
        public TenantedBlobWorkflowStoreFactory(
            ITenantCloudBlobContainerFactory containerFactory,
            BlobStorageContainerDefinition containerDefinition,
            IJsonSerializerSettingsProvider serializerSettingsProvider)
        {
            this.containerFactory = containerFactory
                ?? throw new ArgumentNullException(nameof(containerFactory));
            this.containerDefinition = containerDefinition
                ?? throw new ArgumentNullException(nameof(containerDefinition));
            this.serializerSettingsProvider = serializerSettingsProvider
                ?? throw new ArgumentNullException(nameof(serializerSettingsProvider));
        }

        /// <inheritdoc/>
        public async Task<IWorkflowStore> GetWorkflowStoreForTenantAsync(ITenant tenant)
        {
            CloudBlobContainer blobContainer =
                await this.containerFactory.GetBlobContainerForTenantAsync(tenant, this.containerDefinition).ConfigureAwait(false);

            // No need to cache these instances as they are lightweight wrappers around the container.
            return new BlobStorageWorkflowStore(blobContainer, this.serializerSettingsProvider);
        }
    }
}