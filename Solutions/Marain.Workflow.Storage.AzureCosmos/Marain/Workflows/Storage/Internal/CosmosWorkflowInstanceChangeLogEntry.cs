// <copyright file="CosmosWorkflowInstanceChangeLogEntry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Storage.Internal
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// A log entry for a workflow instance change.
    /// </summary>
    internal class CosmosWorkflowInstanceChangeLogEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CosmosWorkflowInstanceChangeLogEntry"/> class.
        /// </summary>
        /// <param name="id">The unique ID of this workflow instance.</param>
        /// <param name="trigger">the trigger that caused the workflow instance to be changed, or null if this was a newly initialized workflow.</param>
        /// <param name="workflowInstance">The workflow instance that has changed.</param>
        /// <param name="timestamp">The timestamp of the change.</param>
        [JsonConstructor]
        public CosmosWorkflowInstanceChangeLogEntry(string id, IWorkflowTrigger trigger, WorkflowInstance workflowInstance, long timestamp)
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
        /// Gets the partition key.
        /// </summary>
        public string PartitionKey => this.WorkflowInstance.Id;

        /// <summary>
        /// Gets the unix timestamp for the log entry.
        /// </summary>
        public long Timestamp { get; }

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
