// <copyright file="AzureBlobWorkflowInstanceChangeLogEntry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Storage.Internal
{
    using System;

    /// <summary>
    /// A log entry for a workflow instance change.
    /// </summary>
    internal class AzureBlobWorkflowInstanceChangeLogEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AzureBlobWorkflowInstanceChangeLogEntry"/> class.
        /// </summary>
        /// <param name="id">The unique ID of this workflow instance.</param>
        /// <param name="trigger">the trigger that caused the workflow instance to be changed, or null if this was a newly initialized workflow.</param>
        /// <param name="workflowInstance">The workflow instance that has changed.</param>
        /// <param name="timestamp">The timestamp of the change.</param>
        public AzureBlobWorkflowInstanceChangeLogEntry(string id, IWorkflowTrigger trigger, WorkflowInstance workflowInstance, int timestamp)
        {
            this.Id = id;
            this.Trigger = trigger;
            this.WorkflowInstance = workflowInstance ?? throw new ArgumentNullException(nameof(workflowInstance));
            this.Timestamp = timestamp;
        }

        /// <summary>
        /// Gets the Unique ID of this workflow instance.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the unix timestamp for the log entry.
        /// </summary>
        public int Timestamp { get; }

        /// <summary>
        /// Gets the trigger that caused the workflow instance to be changed, or null if this was a newly initialized workflow.
        /// </summary>
        public IWorkflowTrigger Trigger { get; }

        /// <summary>
        /// Gets the workflow instance that has changed.
        /// </summary>
        public WorkflowInstance WorkflowInstance { get; }
    }
}
