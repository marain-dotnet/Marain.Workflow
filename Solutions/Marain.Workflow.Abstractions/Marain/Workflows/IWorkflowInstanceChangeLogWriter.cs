// <copyright file="IWorkflowInstanceChangeLogWriter.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the interface for writing log records describing the history of workflow instance changes.
    /// </summary>
    public interface IWorkflowInstanceChangeLogWriter
    {
        /// <summary>
        /// Creates a workflow instance change record.
        /// </summary>
        /// <param name="trigger">The trigger that caused the update to the workflow instance, from which to create a snapshot; or null if this was an initialization operation for the workflow instance.</param>
        /// <param name="instance">The workflow instance in its updated state, from which to create a snapshot.</param>
        /// <param name="partitionKey">The partition key for the record.</param>
        /// <returns>A <see cref="Task"/> that completes once the record has been stored.</returns>
        /// <remarks>
        /// <para>The log entry consists of a snapshot of the trigger that caused the state change in the instance, and a snapshot of the workflow instance in that new state.</para>
        /// </remarks>
        Task CreateLogEntryAsync(IWorkflowTrigger trigger, WorkflowInstance instance, string partitionKey = null);
    }
}
