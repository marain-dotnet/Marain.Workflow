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
        /// <param name="description">The description of the workflow.</param>
        public Workflow(string id = null, string displayName = null, string description = null)
        {
            this.Id = string.IsNullOrEmpty(id) ? Guid.NewGuid().ToString() : id;
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
            get => this.states ?? (this.states = new Dictionary<string, WorkflowState>());

            set => this.states = value;
        }

        /// <summary>
        /// Gets or sets the list of subscriptions for events raised as a result of activity against instances of this
        /// workflow.
        /// </summary>
        public IList<WorkflowEventSubscription> WorkflowEventSubscriptions
        {
            get => this.eventSubscriptions ?? (this.eventSubscriptions = new List<WorkflowEventSubscription>());

            set => this.eventSubscriptions = value;
        }

        /// <summary>
        /// Adds a new state to the workflow.
        /// </summary>
        /// <param name="state">The new state to add.</param>
        public void AddState(WorkflowState state)
        {
            if (!this.States.ContainsKey(state.Id))
            {
                this.States.Add(state.Id, state);
            }
        }

        /// <summary>
        /// Adds a new state to the workflow.
        /// </summary>
        /// <param name="id">The id for the state.</param>
        /// <param name="displayName">The displayname of the state.</param>
        /// <param name="description">The description of the state.</param>
        /// <returns>The newly created state.</returns>
        public WorkflowState CreateState(
            string id = null,
            string displayName = null,
            string description = null)
        {
            var state = new WorkflowState { Description = description, DisplayName = displayName };

            if (!string.IsNullOrEmpty(id))
            {
                state.Id = id;
            }

            if (!this.States.ContainsKey(state.Id))
            {
                this.States.Add(state.Id, state);
            }

            return state;
        }

        /// <summary>
        /// Gets the <see cref="WorkflowState" /> that has been defined as the initial state of the workflow.
        /// </summary>
        /// <returns>
        /// The <see cref="WorkflowState" /> that will be the first state for new <see cref="WorkflowInstance" />s
        /// created from this Workflow.
        /// </returns>
        public WorkflowState GetInitialState()
        {
            return this.GetState(this.InitialStateId);
        }

        /// <summary>
        /// Gets the <see cref="WorkflowState" /> with the given Id.
        /// </summary>
        /// <param name="id">The id of the state to retrieve.</param>
        /// <returns>
        /// The <see cref="WorkflowState" /> with the given Id, or null if no matching state is present
        /// in the <see cref="States" /> collection.
        /// </returns>
        public WorkflowState GetState(string id)
        {
            if (this.States.TryGetValue(id, out WorkflowState value))
            {
                return value;
            }

            return null;
        }

        /// <summary>
        /// Removes the <see cref="WorkflowState" /> with the given Id from the.
        /// </summary>
        /// <param name="state">The <see cref="WorkflowState" /> to remove.</param>
        public void RemoveState(WorkflowState state)
        {
            this.States.Remove(state.Id);
        }

        /// <summary>
        /// Adds the given <see cref="WorkflowState" /> to the <see cref="States" /> collection
        /// and sets <see cref="InitialStateId" /> to it's Id.
        /// </summary>
        /// <param name="state">The state to add.</param>
        public void SetInitialState(WorkflowState state)
        {
            this.AddState(state);
            this.InitialStateId = state.Id;
        }
    }
}