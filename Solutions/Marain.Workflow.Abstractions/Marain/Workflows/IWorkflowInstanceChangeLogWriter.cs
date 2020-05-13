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
        /// <param name="trigger">A snapshot of the trigger that caused the change in the workflow instance, or null if this was an initialization.</param>
        /// <param name="instance">A snapshot of the workflow instance that has changed.</param>
        /// <param name="partitionKey">The partition key for the record.</param>
        /// <returns>A <see cref="Task"/> that completes once the record has been stored.</returns>
        Task CreateLogEntryAsync(IWorkflowTrigger trigger, WorkflowInstance instance, string partitionKey = null);
    }
}
