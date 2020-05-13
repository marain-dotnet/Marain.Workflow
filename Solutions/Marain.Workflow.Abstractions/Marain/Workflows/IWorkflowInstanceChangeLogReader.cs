// <copyright file="IWorkflowInstanceChangeLogReader.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the interface for reading a log of the history of workflow instance changes.
    /// </summary>
    public interface IWorkflowInstanceChangeLogReader
    {
        /// <summary>
        /// Gets a page of log entries for a particular workflow instance ID.
        /// </summary>
        /// <param name="workflowInstanceId">The ID of the workflow instance for which to get log entries.</param>
        /// <param name="startingTimestamp">The initial sequence number with which to start enumerating entries.</param>
        /// <param name="maxItems">The maximum number of items per page. Note that fewer than this number may be returned in any page.</param>
        /// <param name="continuationToken">The continuation token supplied from a previous page of results.</param>
        /// <returns>A page of log entires for the given workflow instance.</returns>
        Task<WorkflowInstanceLog> GetLogEntriesAsync(string workflowInstanceId, int? startingTimestamp = null, int maxItems = 25, string continuationToken = null);
    }
}
