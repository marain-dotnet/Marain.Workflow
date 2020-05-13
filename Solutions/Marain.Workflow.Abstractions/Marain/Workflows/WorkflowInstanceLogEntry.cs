// <copyright file="WorkflowInstanceLogEntry.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    /// <summary>
    /// An entry in a <see cref="WorkflowInstanceLogPage"/>, associating a version of a <see cref="WorkflowInstance"/>
    /// with the <see cref="IWorkflowTrigger"/> that caused the instance to change state.
    /// </summary>
    public class WorkflowInstanceLogEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowInstanceLogEntry"/> class.
        /// </summary>
        /// <param name="trigger">The trigger that caused the change, or null if the instance was being initialized.</param>
        /// <param name="instance">The associated version of the workflow instance.</param>
        /// <param name="timestamp">The unix timestamp of the log entry at a resolution of seconds.</param>
        public WorkflowInstanceLogEntry(IWorkflowTrigger trigger, WorkflowInstance instance, int timestamp)
        {
            this.Trigger = trigger;
            this.Instance = instance ?? throw new System.ArgumentNullException(nameof(instance));
            this.Timestamp = timestamp;
        }

        /// <summary>
        /// Gets the trigger that caused the change, or null if the instance was being initialized.
        /// </summary>
        public IWorkflowTrigger Trigger { get; }

        /// <summary>
        /// Gets the associated version of the workflow instance.
        /// </summary>
        public WorkflowInstance Instance { get; }

        /// <summary>
        /// Gets the unix timestamp of the log entry at a resolution of seconds.
        /// </summary>
        public int Timestamp { get; }
    }
}
