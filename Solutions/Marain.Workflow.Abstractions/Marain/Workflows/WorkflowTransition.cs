// <copyright file="WorkflowTransition.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a way of moving between states in a workflow.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A <see cref="WorkflowState" /> can have any number of transitions attached to it. A state
    /// with no transitions is classed as an "end" state, as there is no way of continuing
    /// the workflow from that state.
    /// </para>
    /// <para>
    /// Transitions have <see cref="IWorkflowCondition" /> and <see cref="IWorkflowAction" /> associated
    /// with them. When determining if a transition should be applied for a specific <see cref="WorkflowInstance" />,
    /// the conditions will be checked in conjunction with those of the current and target state. For
    /// more information on this, see docs for <see cref="WorkflowEngine" />.
    /// </para>
    /// <para>
    /// When adding transitions to an <see cref="WorkflowState" /> bear in mind that the transitions will
    /// be checked in order and the first whose conditions are all valid will be selected. This means that
    /// if you have multiple transitions that have similar lists of conditions, you should add the more
    /// specific transitions first.
    /// </para>
    /// </remarks>
    public class WorkflowTransition
    {
        /// <summary>
        /// The content type that will be used when serializing/deserializing.
        /// </summary>
        public const string RegisteredContentType = "application/vnd.marain.workflows.transition";

        /// <summary>
        /// The list of actions.
        /// </summary>
        private IList<IWorkflowAction> actions;

        /// <summary>
        /// The list of conditions.
        /// </summary>
        private IList<IWorkflowCondition> conditions;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowTransition" /> class.
        /// </summary>
        /// <remarks>
        /// You typically do not create instances of <see cref="WorkflowTransition" /> directly. Instead,
        /// use the <see cref="WorkflowState.CreateTransition(WorkflowState, string, string, string)" /> extension
        /// method to create instances.
        /// </remarks>
        public WorkflowTransition()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Gets or sets the list of actions that will be executed to action this transition.
        /// </summary>
        /// <remarks>
        /// Actions will be executed sequentially and in order.
        /// </remarks>
        public IList<IWorkflowAction> Actions
        {
            get => this.actions ??= new List<IWorkflowAction>();

            set => this.actions = value;
        }

        /// <summary>
        /// Gets or sets the list conditions that must be true in order for this transition
        /// to be valid.
        /// </summary>
        public IList<IWorkflowCondition> Conditions
        {
            get => this.conditions ??= new List<IWorkflowCondition>();

            set => this.conditions = value;
        }

        /// <summary>
        /// Gets the content type that will be used when serializing/deserializing.
        /// </summary>
        public string ContentType => RegisteredContentType;

        /// <summary>
        /// Gets or sets the description of the state.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the display name of the state.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the id for this state.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the Id of the target state for this transition.
        /// </summary>
        public string TargetStateId { get; set; }

        /// <summary>
        /// Gets the list of interests for this state.
        /// </summary>
        /// <param name="instance">
        /// The workflow instance that the state belongs to.
        /// </param>
        /// <returns>
        /// The list of interests for the state.
        /// </returns>
        /// <remarks>
        /// <para>
        /// The list of interests is built up by concatenating the interests
        /// from the <see cref="Conditions" />.
        /// </para>
        /// </remarks>
        public IEnumerable<string> GetInterests(WorkflowInstance instance)
        {
            return this.Conditions.SelectMany(c => c.GetInterests(instance)).ToList();
        }
    }
}