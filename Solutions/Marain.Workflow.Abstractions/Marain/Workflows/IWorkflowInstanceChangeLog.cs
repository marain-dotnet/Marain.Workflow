// <copyright file="IWorkflowInstanceChangeLog.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the interface logging the history of workflow instance changes.
    /// </summary>
    public interface IWorkflowInstanceChangeLog
    {
        /// <summary>
        /// Creates a workflow instance change record.
        /// </summary>
        /// <param name="trigger">The trigger that caused the change in the workflow instance, or null if this was an initialization.</param>
        /// <param name="instance">The workflow instance that has changed.</param>
        /// <param name="partitionKey">The partition key for the instance.</param>
        /// <returns>A <see cref="Task"/> that completes once the instance has been stored.</returns>
        Task CreateLogEntryAsync(IWorkflowTrigger trigger, WorkflowInstance instance, string partitionKey = null);

        /// <summary>
        /// Gets a page of log entries for a particular workflow instance ID.
        /// </summary>
        /// <param name="workflowInstanceId">The ID of the workflow instance for which to get log entries.</param>
        /// <param name="startingSequenceNumber">The initial sequence number with which to start enumerating entries.</param>
        /// <param name="maxItems">The maximum number of items per page. Note that fewer than this number may be returned in any page.</param>
        /// <param name="continuationToken">The continuation token supplied from a previous page of results.</param>
        /// <returns>A page of log entires for the given workflow instance.</returns>
        Task<WorkflowInstanceLog> GetLogEntriesAsync(string workflowInstanceId, ulong? startingSequenceNumber = null, int maxItems = 25, string continuationToken = null);
    }
}
