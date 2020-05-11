// <copyright file="CosmosWorkflowInstanceChangeLog.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Storage
{
    using System;
    using System.Threading.Tasks;
    using Corvus.Retry;
    using Marain.Workflows.Storage.Internal;
    using Microsoft.Azure.Cosmos;

    /// <summary>
    /// A CosmosDb implementation of the workflow instance change log.
    /// </summary>
    public class CosmosWorkflowInstanceChangeLog : IWorkflowInstanceChangeLog
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
        public async Task CreateLogEntryAsync(IWorkflowTrigger trigger, WorkflowInstance workflowInstance, string partitionKey = null)
        {
            if (workflowInstance == null)
            {
                throw new ArgumentNullException(nameof(workflowInstance));
            }

            var logEntry = new WorkflowInstanceChangeLogEntry(trigger, workflowInstance);
            await Retriable.RetryAsync(() =>
                this.Container.UpsertItemAsync(
                    logEntry,
                    new PartitionKey(partitionKey ?? workflowInstance.Id)))
                .ConfigureAwait(false);
        }
    }
}
