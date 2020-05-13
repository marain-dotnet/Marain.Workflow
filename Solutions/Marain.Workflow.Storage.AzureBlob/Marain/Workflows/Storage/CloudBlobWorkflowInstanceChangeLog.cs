// <copyright file="CloudBlobWorkflowInstanceChangeLog.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Storage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Corvus.Extensions;
    using Corvus.Extensions.Json;
    using Corvus.Retry;
    using Marain.Workflows;
    using Marain.Workflows.Storage.Internal;
    using Microsoft.Azure.Storage;
    using Microsoft.Azure.Storage.Blob;
    using Newtonsoft.Json;

    /// <summary>
    /// A CosmosDb implementation of the workflow instance change log.
    /// </summary>
    public class CloudBlobWorkflowInstanceChangeLog : IWorkflowInstanceChangeLogWriter, IWorkflowInstanceChangeLogReader
    {
        private const string RecordTimestampKey = "recordtimestamp";
        private readonly IJsonSerializerSettingsProvider serializerSettingsProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudBlobWorkflowInstanceChangeLog"/> class.
        /// </summary>
        /// <param name="serializerSettingsProvider">The serializer settings provider for the store.</param>
        /// <param name="container">The cloud blob container in which to store the log.</param>
        public CloudBlobWorkflowInstanceChangeLog(IJsonSerializerSettingsProvider serializerSettingsProvider, CloudBlobContainer container)
        {
            this.serializerSettingsProvider = serializerSettingsProvider;
            this.Container = container;
        }

        /// <summary>
        /// Gets the <see cref="CloudBlobContainer"/> for the log.
        /// </summary>
        public CloudBlobContainer Container { get; }

        /// <inheritdoc/>
        public Task RecordWorkflowInstanceChangeAsync(IWorkflowTrigger trigger, WorkflowInstance workflowInstance, string partitionKey = null)
        {
            return Retriable.RetryAsync(() =>
                this.CreateLogEntryCoreAsync(trigger, workflowInstance));
        }

        /// <inheritdoc/>
        public Task<WorkflowInstanceLogPage> GetLogEntriesAsync(string workflowInstanceId, int? startingTimestamp = null, int maxItems = 25, string continuationToken = null)
        {
            return Retriable.RetryAsync(() =>
                this.GetLogEntriesCoreAsync(workflowInstanceId, startingTimestamp, maxItems, continuationToken));
        }

        private async Task<WorkflowInstanceLogPage> GetLogEntriesCoreAsync(string workflowInstanceId, int? startingTimestamp = null, int maxItems = 25, string continuationToken = null)
        {
            int pageIndex = 0;
            int pageSize = maxItems;

            if (continuationToken != null)
            {
                string serializedToken = continuationToken.FromBase64();
                ContinuationToken token = JsonConvert.DeserializeObject<ContinuationToken>(serializedToken);
                pageIndex = token.PageIndex;
                pageSize = token.PageSize;
                startingTimestamp = token.StartingTimestamp;
            }

            // Don't bother with metadata if we are not filtering by a starting timestamp
            IEnumerable<CloudBlockBlob> blobs = this.Container.ListBlobs($"{workflowInstanceId}/", true, startingTimestamp.HasValue ? BlobListingDetails.Metadata : BlobListingDetails.None).Cast<CloudBlockBlob>();
            if (startingTimestamp.HasValue)
            {
                string timestampString = startingTimestamp.Value.ToString("D10");
                blobs = blobs.Where(b => b.Metadata[RecordTimestampKey].CompareTo(timestampString) >= 0);
            }

            var blobList = blobs.Skip(pageSize * pageIndex).Take(pageSize).ToList();
            var resultSet = new List<WorkflowInstanceLogEntry>();

            foreach (CloudBlockBlob blob in blobList)
            {
                resultSet.Add(await this.GetLogEntry(blob).ConfigureAwait(false));
            }

            return new WorkflowInstanceLogPage(resultSet.Count > 0 ? JsonConvert.SerializeObject(new ContinuationToken(pageSize, pageIndex + 1, startingTimestamp)).AsBase64() : null, resultSet);
        }

        private async Task<WorkflowInstanceLogEntry> GetLogEntry(CloudBlockBlob blob)
        {
            string serializedEntry = await blob.DownloadTextAsync().ConfigureAwait(false);
            CloudBlobWorkflowInstanceChangeLogEntry entry = JsonConvert.DeserializeObject<CloudBlobWorkflowInstanceChangeLogEntry>(serializedEntry, this.serializerSettingsProvider.Instance);

            return new WorkflowInstanceLogEntry(
                entry.Trigger,
                entry.WorkflowInstance,
                entry.Timestamp);
        }

        private async Task CreateLogEntryCoreAsync(IWorkflowTrigger trigger, WorkflowInstance workflowInstance)
        {
            var logEntry =
                new CloudBlobWorkflowInstanceChangeLogEntry(
                    Guid.NewGuid().ToString(),
                    trigger,
                    workflowInstance,
                    (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds());

            string serializedLogEntry = JsonConvert.SerializeObject(logEntry, this.serializerSettingsProvider.Instance);

            CloudBlockBlob blob = this.Container.GetBlockBlobReference($"{workflowInstance.Id}/{logEntry.Timestamp:D10}-{logEntry.Id}.json");

            await blob.UploadTextAsync(serializedLogEntry, Encoding.UTF8, new AccessCondition { IfNoneMatchETag = "*" }, null, null).ConfigureAwait(false);
            blob.Properties.ContentType = "application/json";
            await blob.SetPropertiesAsync().ConfigureAwait(false);
            blob.Metadata.Add(RecordTimestampKey, logEntry.Timestamp.ToString("D10"));
            await blob.SetMetadataAsync().ConfigureAwait(false);
        }
    }
}
