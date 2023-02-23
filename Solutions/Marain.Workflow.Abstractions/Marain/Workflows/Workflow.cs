// <copyright file="Workflow.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// Class representing the definition of a workflow. Contains the list of possible states and potential
    /// transitions between them, along with the actions associated with moving between states.
    /// </summary>
    public class Workflow
    {
        /// <summary>
        /// The registered content type used when this object is serialized/deserialized.
        /// </summary>
        public const string RegisteredContentType = "application/vnd.marain.workflows.workflow";

        /// <summary>
        /// The list of possible states for the workflow.
        /// </summary>
        private IDictionary<string, WorkflowState> states;

        private IList<WorkflowEventSubscription> eventSubscriptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="Workflow" /> class.
        /// </summary>
        public Workflow()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Workflow"/> class.
        /// </summary>
        /// <param name="id">The id of the workflow.</param>
        /// <param name="displayName">The display name of the workflow.</param>
        /// <param name="states"></param>
        /// <param name="description">The description of the workflow.</param>
        /// <param name="eTag"></param>
        /// <param name="initialStateId"></param>
        /// <param name="workflowEventSubscriptions"></param>
        public Workflow(
            string id,
            IDictionary<string, WorkflowState> states,
            string initialStateId,
            string displayName = null,
            string description = null,
            string eTag = null,
            IList<WorkflowEventSubscription> workflowEventSubscriptions = null)
        {
            // Verify that States contan initial state
            this.Id = id;
            this.DisplayName = displayName;
            this.Description = description;
        }

        /// <summary>
        /// Gets the content type used when this object is serialized/deserialized.
        /// </summary>
        public string ContentType => RegisteredContentType;

        /// <summary>
        /// Gets or sets the description of this workflow.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the display name of this workflow.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the unique id of the workflow.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets the parition key for the workflow.
        /// </summary>
        public string PartitionKey => this.Id;

        /// <summary>
        /// Gets or sets the ETag of the workflow.
        /// </summary>
        [JsonProperty("_etag")]
        public string ETag { get; set; }

        /// <summary>
        /// Gets or sets the id of this workflow's initial State.
        /// </summary>
        /// <remarks>
        /// Do not set directly; prefer calling <see cref="SetInitialState(WorkflowState)" />.
        /// When new instances of the workflow are created, they will enter this state and any entry
        /// conditions and actions on the state will be validated/executed. Failure in any condition
        /// or action will result in the new workflow instance being Faulted.
        /// </remarks>
        public string InitialStateId { get; set; }

        /// <summary>
        /// Gets or sets list of possible states for the workflow.
        /// </summary>
        /// <remarks>
        /// Use <see cref="CreateState(string, string, string)" /> or <see cref="AddState(WorkflowState)"/>
        /// to add new states, and <see cref="GetState" /> and <see cref="RemoveState" /> to retrieve/remove states.
        /// </remarks>
        public IDictionary<string, WorkflowState> States
        {
            get => this.states ??= new Dictionary<string, WorkflowState>();

            set => this.states = value;
        }

        /// <summary>
        /// Gets or sets the list of subscriptions for events raised as a result of activity against instances of this
        /// workflow.
        /// </summary>
        public IList<WorkflowEventSubscription> WorkflowEventSubscriptions
        {
            get => this.eventSubscriptions ??= new List<WorkflowEventSubscription>();

            set => this.eventSubscriptions = value;
        }
    }
}