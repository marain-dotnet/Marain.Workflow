// <copyright file="CosmosWorkflowInstanceLogEntry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Storage.Internal
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// A log entry for a workflow instance change.
    /// </summary>
    internal class CosmosWorkflowInstanceLogEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CosmosWorkflowInstanceLogEntry"/> class.
        /// </summary>
        /// <param name="trigger">the trigger that caused the workflow instance to be changed, or null if this was a newly initialized workflow.</param>
        /// <param name="workflowInstance">The workflow instance that has changed.</param>
        /// <param name="timestamp">The timestamp of the change log entry.</param>
        [JsonConstructor]
        public CosmosWorkflowInstanceLogEntry(IWorkflowTrigger trigger, WorkflowInstance workflowInstance, int? timestamp = null)
        {
            this.Trigger = trigger;
            this.WorkflowInstance = workflowInstance ?? throw new ArgumentNullException(nameof(workflowInstance));
            this.Timestamp = timestamp;
        }

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

        /// <summary>
        /// Gets the timestamp of the log entry.
        /// </summary>
        public int? Timestamp { get; }
    }
}
