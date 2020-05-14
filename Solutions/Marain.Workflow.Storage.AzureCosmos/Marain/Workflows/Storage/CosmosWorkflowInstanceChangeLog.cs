// <copyright file="CosmosWorkflowInstanceChangeLog.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Storage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Corvus.Retry;
    using Marain.Workflows;
    using Marain.Workflows.Storage.Internal;
    using Microsoft.Azure.Cosmos;

    /// <summary>
    /// A CosmosDb implementation of the workflow instance change log.
    /// </summary>
    public class CosmosWorkflowInstanceChangeLog : IWorkflowInstanceChangeLogWriter, IWorkflowInstanceChangeLogReader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CosmosWorkflowInstanceChangeLog"/> class.
        /// </summary>
        /// <param name="workflowInstanceChangeLogContainer">The repository in which to store workflow instances change log entries.</param>
        public CosmosWorkflowInstanceChangeLog(
            Container workflowInstanceChangeLogContainer)
        {
            this.Container = workflowInstanceChangeLogContainer;
        }

        /// <summary>
        /// Gets the underlying Cosmos <see cref="Container"/> for this workflow instance change log.
        /// </summary>
        public Container Container { get; }

        /// <inheritdoc/>
        public async Task RecordWorkflowInstanceChangeAsync(IWorkflowTrigger trigger, WorkflowInstance workflowInstance)
        {
            if (workflowInstance == null)
            {
                throw new ArgumentNullException(nameof(workflowInstance));
            }

            var logEntry = new CosmosWorkflowInstanceChangeLogEntry(Guid.NewGuid().ToString(), trigger, workflowInstance, DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
            await Retriable.RetryAsync(() =>
                this.Container.UpsertItemAsync(
                    logEntry,
                    new PartitionKey(workflowInstance.Id)))
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<WorkflowInstanceLogPage> GetLogEntriesAsync(string workflowInstanceId, long? startingTimestamp = null, int maxItems = 25, string continuationToken = null)
        {
            var query = new StringBuilder("SELECT * FROM log l WHERE l.workflowInstance.id = @workflowInstanceId");
            if (startingTimestamp.HasValue)
            {
                query.Append(" AND l.timestamp >= @startingTimestamp");
            }

            query.Append(" ORDER BY l.timestamp ASC");

            QueryDefinition queryDefinition =
            new QueryDefinition(query.ToString())
                .WithParameter("@workflowInstanceId", workflowInstanceId);

            if (startingTimestamp.HasValue)
            {
                queryDefinition = queryDefinition.WithParameter("@startingTimestamp", startingTimestamp);
            }

            string nextToken = null;
            IEnumerable<WorkflowInstanceLogEntry> results = Enumerable.Empty<WorkflowInstanceLogEntry>();

            FeedIterator<CosmosWorkflowInstanceChangeLogEntry> iterator = this.Container.GetItemQueryIterator<CosmosWorkflowInstanceChangeLogEntry>(queryDefinition, continuationToken, new QueryRequestOptions { MaxItemCount = maxItems });
            if (iterator.HasMoreResults)
            {
                FeedResponse<CosmosWorkflowInstanceChangeLogEntry> response = await iterator.ReadNextAsync().ConfigureAwait(false);
                nextToken = response.ContinuationToken;
                results = response.Select(l => new WorkflowInstanceLogEntry(l.Trigger, l.WorkflowInstance, l.Timestamp)).ToList();
            }

            return new WorkflowInstanceLogPage(nextToken, results);
        }
    }
}
