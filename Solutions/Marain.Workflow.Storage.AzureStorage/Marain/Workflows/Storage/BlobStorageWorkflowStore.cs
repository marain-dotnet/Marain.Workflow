// <copyright file="BlobStorageWorkflowStore.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Storage
{
    using System;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using Corvus.Extensions.Json;
    using Microsoft.Azure.Storage;
    using Microsoft.Azure.Storage.Blob;
    using Newtonsoft.Json;

    /// <summary>
    /// A blob storage implementation of the workflow store.
    /// </summary>
    public class BlobStorageWorkflowStore : IWorkflowStore
    {
        private JsonSerializerSettings serializerSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobStorageWorkflowStore"/> class.
        /// </summary>
        /// <param name="workflowContainer">The container in which to store workflows.</param>
        /// <param name="serializerSettingsProvider">The current <see cref="IJsonSerializerSettingsProvider"/>.</param>
        public BlobStorageWorkflowStore(
            CloudBlobContainer workflowContainer,
            IJsonSerializerSettingsProvider serializerSettingsProvider)
        {
            this.Container = workflowContainer;
            this.serializerSettings = serializerSettingsProvider.Instance;
        }

        /// <summary>
        /// Gets the underlying <see cref="CloudBlobContainer"/> for this workflow store.
        /// </summary>
        public CloudBlobContainer Container { get; }

        /// <inheritdoc/>
        public async Task<Workflow> GetWorkflowAsync(string workflowId, string partitionKey = null)
        {
            try
            {
                CloudBlockBlob blob = this.Container.GetBlockBlobReference(workflowId);

                string text = await blob.DownloadTextAsync(Encoding.UTF8, null, null, null).ConfigureAwait(false);
                Workflow workflow = JsonConvert.DeserializeObject<Workflow>(text, this.serializerSettings);
                workflow.ETag = blob.Properties.ETag;
                return workflow;
            }
            catch (StorageException ex) when (ex.RequestInformation.HttpStatusCode == (int)HttpStatusCode.NotFound)
            {
                throw new WorkflowNotFoundException($"The workflow with id {workflowId} was not found", ex);
            }
        }

        /// <inheritdoc/>
        public async Task UpsertWorkflowAsync(Workflow workflow, string partitionKey = null)
        {
            if (workflow == null)
            {
                throw new ArgumentNullException(nameof(workflow));
            }

            try
            {
                CloudBlockBlob blob = this.Container.GetBlockBlobReference(workflow.Id);
                string text = JsonConvert.SerializeObject(workflow, this.serializerSettings);

                AccessCondition accessCondition = string.IsNullOrEmpty(workflow.ETag)
                    ? AccessCondition.GenerateIfNoneMatchCondition("*")
                    : AccessCondition.GenerateIfMatchCondition(workflow.ETag);

                await blob.UploadTextAsync(text, Encoding.UTF8, accessCondition, null, null).ConfigureAwait(false);

                workflow.ETag = blob.Properties.ETag;
            }
            catch (StorageException ex) when (ex.RequestInformation.HttpStatusCode == (int)HttpStatusCode.Conflict)
            {
                throw new WorkflowConflictException();
            }
            catch (StorageException ex) when (ex.RequestInformation.HttpStatusCode == (int)HttpStatusCode.PreconditionFailed)
            {
                throw new WorkflowPreconditionFailedException();
            }
        }
    }
}
