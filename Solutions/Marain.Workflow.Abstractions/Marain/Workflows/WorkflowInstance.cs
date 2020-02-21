// <copyright file="WorkflowInstance.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json;

    /// <summary>
    /// The workflow instance.
    /// </summary>
    public class WorkflowInstance
    {
        /// <summary>
        /// The registered content type used when this object is serialized/deserialized.
        /// </summary>
        public const string RegisteredContentType = "application/vnd.marain.workflows.workflowinstance";

        /// <summary>
        /// Context dictionary for the workflow. This can be used to store any useful data
        /// needed by workflow states to simplify execution of their triggers and actions.
        /// </summary>
        private IDictionary<string, string> context;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowInstance" /> class.
        /// </summary>
        public WorkflowInstance()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Status = WorkflowStatus.Initializing;
        }

        /// <summary>
        /// Gets the registered content type used when this object is serialized/deserialized.
        /// </summary>
        public string ContentType => RegisteredContentType;

        /// <summary>
        /// Gets or sets a dictionary of useful related pieces of data to be stored with
        /// the workflow instance.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Workflow instances will almost always have related data that are stored independently
        /// of the instance itself. However, to make processing triggers more efficient, you
        /// can choose to add specific pieces of that data.
        /// </para>
        /// </remarks>
        public IDictionary<string, string> Context
        {
            get => this.context ?? (this.context = new Dictionary<string, string>());

            set => this.context = value;
        }

        /// <summary>
        /// Gets or sets the unique Id of this workflow instance.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the ETag of this workflow instance.
        /// </summary>
        public string ETag { get; set; }

        /// <summary>
        /// Gets the partition key.
        /// </summary>
        public string PartitionKey => this.Id;

        /// <summary>
        /// Gets or sets the list of interests for this workflow instance.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This list is automatically updated whenever the instance's current <see cref="StateId" />
        /// changes. The list will be populated by calling <see cref="WorkflowState.GetInterests" />
        /// on the current state and  will always include the value of the <see cref="Id" /> property.
        /// </para>
        /// <para>
        /// This list is intended for use by the <see cref="IWorkflowInstanceStore.GetMatchingWorkflowInstanceIdsForSubjectsAsync(IEnumerable{string}, int, int)" />
        /// method to search for <see cref="WorkflowInstance" />s whose interests match at least one of the
        /// current trigger's subjects (see <see cref="IWorkflowTrigger.GetSubjects" />).
        /// </para>
        /// </remarks>
        public IEnumerable<string> Interests { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets a value indicating whether the instance has changed since it was loaded.
        /// </summary>
        [JsonIgnore]
        public bool IsDirty { get; set; }

        /// <summary>
        /// Gets or sets the Id of the current <see cref="WorkflowState" /> for this instance.
        /// </summary>
        /// <remarks>
        /// Note that you should typically set the state using the <see cref="SetState(WorkflowState)"/> method
        /// to ensure that interests are updated for the new state.
        /// </remarks>
        public string StateId { get; set; }

        /// <summary>
        /// Gets or sets the status of this instance. A <see cref="WorkflowInstance" /> can only process
        /// new triggers if it's in the <see cref="WorkflowStatus.Waiting" /> status.
        /// </summary>
        public WorkflowStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the Id of the <see cref="Workflow" /> that this is an instance of.
        /// </summary>
        public string WorkflowId { get; set; }

        /// <summary>
        /// Sets the current state of the <see cref="WorkflowInstance" /> to the specified state.
        /// </summary>
        /// <param name="targetState">
        /// The target state.
        /// </param>
        /// <remarks>
        /// This will also result in the <see cref="Interests" /> collection being updated from
        /// the new state via a call to <see cref="WorkflowState.GetInterests" />.
        /// </remarks>
        public void SetState(WorkflowState targetState)
        {
            this.StateId = targetState.Id;
            this.UpdateInterests(targetState);
        }

        /// <summary>
        /// Updates the collection of <see cref="Interests" /> for this workflow instance from
        /// the current <see cref="WorkflowState" />.
        /// </summary>
        /// <param name="state">The <see cref="WorkflowState"/> from whichto get interests.</param>
        private void UpdateInterests(WorkflowState state)
        {
            this.Interests = state.GetInterests(this).Concat(new[] { this.Id }).ToList();
        }
    }
}