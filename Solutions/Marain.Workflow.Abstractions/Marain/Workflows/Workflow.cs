// <copyright file="Workflow.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

#nullable enable

namespace Marain.Workflows
{
    using System;
    using System.Collections.Generic;

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
        /// Initializes a new instance of the <see cref="Workflow"/> class.
        /// </summary>
        /// <param name="id">The <see cref="Id"/> property.</param>
        /// <param name="states">The <see cref="States"/> property.</param>
        /// <param name="initialStateId">The <see cref="InitialStateId"/> property.</param>
        /// <param name="displayName">The <see cref="DisplayName"/> property.</param>
        /// <param name="description">The <see cref="Description"/> property.</param>
        /// <param name="workflowEventSubscriptions">The <see cref="WorkflowEventSubscriptions"/> property.</param>
        public Workflow(
            string id,
            IReadOnlyDictionary<string, WorkflowState> states,
            string initialStateId,
            string? displayName = null,
            string? description = null,
            IReadOnlyList<WorkflowEventSubscription>? workflowEventSubscriptions = null)
        {
            this.Id = id;
            this.States = states;
            this.InitialStateId = initialStateId;
            this.DisplayName = displayName;
            this.Description = description;
            this.WorkflowEventSubscriptions = workflowEventSubscriptions ?? Array.Empty<WorkflowEventSubscription>();

            if (!this.States.ContainsKey(initialStateId))
            {
                throw new ArgumentException(
                    $"Initial state {initialStateId} was not found in {nameof(states)}",
                    nameof(initialStateId));
            }
        }

        /// <summary>
        /// Gets the content type used when this object is serialized/deserialized.
        /// </summary>
        public string ContentType => RegisteredContentType;

        /// <summary>
        /// Gets the description of this workflow.
        /// </summary>
        public string? Description { get; }

        /// <summary>
        /// Gets the display name of this workflow.
        /// </summary>
        public string? DisplayName { get; }

        /// <summary>
        /// Gets the unique id of the workflow.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the id of this workflow's initial State.
        /// </summary>
        /// <remarks>
        /// When new instances of the workflow are created, they will enter this state and any entry
        /// conditions and actions on the state will be validated/executed. Failure in any condition
        /// or action will result in the new workflow instance being Faulted.
        /// </remarks>
        public string InitialStateId { get; }

        /// <summary>
        /// Gets list of possible states for the workflow.
        /// </summary>
        public IReadOnlyDictionary<string, WorkflowState> States { get; }

        /// <summary>
        /// Gets the list of subscriptions for events raised as a result of activity against instances of this
        /// workflow.
        /// </summary>
        public IReadOnlyList<WorkflowEventSubscription> WorkflowEventSubscriptions { get; }
    }
}