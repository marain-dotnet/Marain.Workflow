// <copyright file="BlobStorageWorkflowStore.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Storage
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    using Azure;
    using Azure.Storage.Blobs;
    using Azure.Storage.Blobs.Models;
    using Azure.Storage.Blobs.Specialized;

    using Corvus.Extensions.Json;
    using Corvus.Storage;
    using Newtonsoft.Json;

    /// <summary>
    /// A blob storage implementation of the workflow store.
    /// </summary>
    public class BlobStorageWorkflowStore : IWorkflowStore
    {
        private static readonly Encoding UTF8WithoutBom = new UTF8Encoding(false);
        private readonly JsonSerializer jsonSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobStorageWorkflowStore"/> class.
        /// </summary>
        /// <param name="workflowContainer">The container in which to store workflows.</param>
        /// <param name="serializerSettingsProvider">The current <see cref="IJsonSerializerSettingsProvider"/>.</param>
        public BlobStorageWorkflowStore(
            BlobContainerClient workflowContainer,
            IJsonSerializerSettingsProvider serializerSettingsProvider)
        {
            this.Container = workflowContainer;
            this.jsonSerializer = JsonSerializer.Create(serializerSettingsProvider.Instance);
        }

        /// <summary>
        /// Gets the underlying <see cref="BlobContainerClient"/> for this workflow store.
        /// </summary>
        public BlobContainerClient Container { get; }

        /// <inheritdoc/>
        public async Task<EntityWithETag<Workflow>> GetWorkflowAsync(string workflowId, string partitionKey = null, string eTag)
        {
            BlockBlobClient blob = this.Container.GetBlockBlobClient(workflowId);
            Response<BlobDownloadResult> response;
            try
            {
                response = await blob.DownloadContentAsync().ConfigureAwait(false);
            }
            catch (RequestFailedException ex) when (ex.Status == (int)HttpStatusCode.NotFound)
            {
                throw new WorkflowNotFoundException($"The workflow with id {workflowId} was not found", ex);
            }

            Workflow workflow;
            using (StreamReader sr = new(response.Value.Content.ToStream(), leaveOpen: false))
            using (JsonTextReader jr = new(sr))
            {
                workflow = this.jsonSerializer.Deserialize<Workflow>(jr);
            }

            string eTag = response.Value.Details.ETag.ToString("H");
            return new EntityWithETag<Workflow>(workflow, eTag);
        }

        /// <inheritdoc/>
        public async Task<string> UpsertWorkflowAsync(Workflow workflow, string partitionKey = null, string eTag = null)
        {
            if (workflow == null)
            {
                throw new ArgumentNullException(nameof(workflow));
            }

            BlockBlobClient blob = this.Container.GetBlockBlobClient(workflow.Id);
            MemoryStream content = new();
            using (var sw = new StreamWriter(content, UTF8WithoutBom, leaveOpen: true))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                this.jsonSerializer.Serialize(writer, workflow);
            }

            content.Position = 0;
            try
            {
                BlobRequestConditions requestConditions = new();
                if (string.IsNullOrEmpty(eTag))
                {
                    requestConditions.IfNoneMatch = ETag.All;
                }
                else
                {
                    requestConditions.IfMatch = new ETag(eTag);
                }

                Response<BlobContentInfo> response = await blob.UploadAsync(
                    content,
                    new BlobUploadOptions { Conditions = requestConditions })
                    .ConfigureAwait(false);
                return response.Value.ETag.ToString("H");
            }
            catch (RequestFailedException ex) when (ex.Status == (int)HttpStatusCode.Conflict)
            {
                throw new WorkflowConflictException();
            }
            catch (RequestFailedException ex) when (ex.Status == (int)HttpStatusCode.PreconditionFailed)
            {
                throw new WorkflowPreconditionFailedException();
            }
        }
    }
}