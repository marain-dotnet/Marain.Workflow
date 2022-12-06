// <copyright file="TenantedBlobWorkflowStoreFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System;
    using System.Threading.Tasks;

    using Azure.Storage.Blobs;

    using Corvus.Extensions.Json;
    using Corvus.Storage.Azure.BlobStorage.Tenancy;
    using Corvus.Tenancy;

    using Marain.Workflows.Storage;

    /// <summary>
    /// Factory class for retrieving Azure blob storage-based instances of <see cref="IWorkflowStore"/> for specific
    /// <see cref="Tenant"/>s.
    /// </summary>
    public class TenantedBlobWorkflowStoreFactory : ITenantedWorkflowStoreFactory
    {
        private readonly IBlobContainerSourceWithTenantLegacyTransition containerFactory;
        private readonly string v2ConfigurationKey;
        private readonly string v3ConfigurationKey;
        private readonly IJsonSerializerSettingsProvider serializerSettingsProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantedBlobWorkflowStoreFactory"/> class.
        /// </summary>
        /// <param name="containerFactory">
        /// The <see cref="IBlobContainerSourceWithTenantLegacyTransition"/> that will be used to
        /// create underlying <see cref="BlobContainerClient"/> instances for the content stores.</param>
        /// <param name="v2ConfigurationKey">
        /// The tenant properties configuration key in which to find V2-style settings.
        /// </param>
        /// <param name="v3ConfigurationKey">
        /// The tenant properties configuration key in which to find V3-style settings.
        /// </param>
        /// <param name="serializerSettingsProvider">The current <see cref="IJsonSerializerSettingsProvider"/>.</param>
        public TenantedBlobWorkflowStoreFactory(
            IBlobContainerSourceWithTenantLegacyTransition containerFactory,
            string v2ConfigurationKey,
            string v3ConfigurationKey,
            IJsonSerializerSettingsProvider serializerSettingsProvider)
        {
            this.containerFactory = containerFactory
                ?? throw new ArgumentNullException(nameof(containerFactory));
            this.v2ConfigurationKey = v2ConfigurationKey
                ?? throw new ArgumentNullException(nameof(v2ConfigurationKey));
            this.v3ConfigurationKey = v3ConfigurationKey;
            this.serializerSettingsProvider = serializerSettingsProvider
                ?? throw new ArgumentNullException(nameof(serializerSettingsProvider));
        }

        /// <inheritdoc/>
        public async Task<IWorkflowStore> GetWorkflowStoreForTenantAsync(ITenant tenant)
        {
            string tenantedLogicalContainerName = AzureStorageBlobTenantedContainerNaming.GetTenantedLogicalBlobContainerNameFor(
                tenant,
                WorkflowAzureBlobTenancyPropertyKeys.StoreDefinitionsLogicalContainerName);
            BlobContainerClient blobContainer =
                await this.containerFactory.GetBlobContainerClientFromTenantAsync(
                    tenant,
                    this.v2ConfigurationKey,
                    this.v3ConfigurationKey,
                    tenantedLogicalContainerName).ConfigureAwait(false);

            // No need to cache these instances as they are lightweight wrappers around the container.
            return new BlobStorageWorkflowStore(blobContainer, this.serializerSettingsProvider);
        }
    }
}