// <copyright file="WorkflowInstanceChangeLogEntry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Storage.Internal
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// A log entry for a workflow instance change.
    /// </summary>
    internal class WorkflowInstanceChangeLogEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowInstanceChangeLogEntry"/> class.
        /// </summary>
        /// <param name="trigger">the trigger that caused the workflow instance to be changed, or null if this was a newly initialized workflow.</param>
        /// <param name="workflowInstance">The workflow instance that has changed.</param>
        public WorkflowInstanceChangeLogEntry(IWorkflowTrigger trigger, WorkflowInstance workflowInstance)
            : this(Guid.NewGuid().ToString(), trigger, workflowInstance)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowInstanceChangeLogEntry"/> class.
        /// </summary>
        /// <param name="id">The unique ID of this workflow instance.</param>
        /// <param name="trigger">the trigger that caused the workflow instance to be changed, or null if this was a newly initialized workflow.</param>
        /// <param name="workflowInstance">The workflow instance that has changed.</param>
        [JsonConstructor]
        public WorkflowInstanceChangeLogEntry(string id, IWorkflowTrigger trigger, WorkflowInstance workflowInstance)
        {
            this.Id = id ?? throw new ArgumentNullException(nameof(trigger));
            this.Trigger = trigger;
            this.WorkflowInstance = workflowInstance ?? throw new ArgumentNullException(nameof(workflowInstance));
        }

        /// <summary>
        /// Gets the Unique ID of this workflow instance.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the partition key.
        /// </summary>
        public string PartitionKey => this.WorkflowInstance.Id;

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
