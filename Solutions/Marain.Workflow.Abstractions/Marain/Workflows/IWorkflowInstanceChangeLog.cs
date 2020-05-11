// <copyright file="IWorkflowInstanceChangeLog.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
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
    }
}
