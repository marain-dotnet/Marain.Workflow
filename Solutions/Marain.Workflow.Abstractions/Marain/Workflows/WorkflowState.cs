// <copyright file="WorkflowState.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Corvus.Extensions;

    /// <summary>
    ///     Represents a single state in a workflow.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         A workflow state contains:
    ///         - Entry conditions: A list of <see cref="IWorkflowCondition" /> that must be
    ///         satisfied in order for a workflow instance to enter the state.
    ///         - Entry actions: A list of <see cref="IWorkflowAction" /> that will be
    ///         executed when a workflow instance enters this state.
    ///         - Exit conditions: A list of <see cref="IWorkflowCondition" /> that must be
    ///         satisfied in order for a workflow instance to leave the state.
    ///         - Exit actions: A list of <see cref="IWorkflowAction" /> that will be
    ///         executed when a workflow instance leaves this state.
    ///         - Transitions: A list of <see cref="WorkflowTransition" /> that can be used
    ///         to move from this state to another (or if necessary, from this state to
    ///         itself.
    ///     </para>
    ///     <para>
    ///         These collections are used by <see cref="IWorkflowEngine" /> to
    ///         determine whether an <see cref="WorkflowInstance" /> in a particular state can process a
    ///         trigger. See remarks on the <see cref="IWorkflowEngine.ProcessTriggerAsync(IWorkflowTrigger, string, string)" />
    ///         extension method for an explanation of how this is done.
    ///     </para>
    /// </remarks>
    public class WorkflowState
    {
        /// <summary>
        ///     The content type that will be used when serializing/deserializing.
        /// </summary>
        public const string RegisteredContentType = "application/vnd.marain.workflows.state";

        /// <summary>
        ///     The list of entry actions.
        /// </summary>
        private List<IWorkflowAction> entryActions;

        /// <summary>
        ///     The list of entry conditions.
        /// </summary>
        private List<IWorkflowCondition> entryConditions;

        /// <summary>
        ///     The list of exit actions.
        /// </summary>
        private List<IWorkflowAction> exitActions;

        /// <summary>
        ///     The list of exit conditions.
        /// </summary>
        private List<IWorkflowCondition> exitConditions;

        /// <summary>
        ///     The list of transitions.
        /// </summary>
        private List<WorkflowTransition> transitions;

        /// <summary>
        ///     Initializes a new instance of the <see cref="WorkflowState" /> class.
        /// </summary>
        public WorkflowState()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        /// <summary>
        ///     Gets the content type that will be used when serializing/deserializing.
        /// </summary>
        public string ContentType => RegisteredContentType;

        /// <summary>
        ///     Gets or sets the description of the state.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Gets or sets the display name of the state.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        ///     Gets the list of <see cref="IWorkflowAction" />s that will be executed when entering this state.
        /// </summary>
        public List<IWorkflowAction> EntryActions
        {
            get => this.entryActions ?? (this.entryActions = new List<IWorkflowAction>());

            private set => this.entryActions = value;
        }

        /// <summary>
        ///     Gets the list of <see cref="IWorkflowCondition" />s that must be true in order to enter this state.
        /// </summary>
        public List<IWorkflowCondition> EntryConditions
        {
            get => this.entryConditions ?? (this.entryConditions = new List<IWorkflowCondition>());

            private set => this.entryConditions = value;
        }

        /// <summary>
        ///     Gets the list of <see cref="IWorkflowAction" />s that will be executed when leaving this state.
        /// </summary>
        public List<IWorkflowAction> ExitActions
        {
            get => this.exitActions ?? (this.exitActions = new List<IWorkflowAction>());

            private set => this.exitActions = value;
        }

        /// <summary>
        ///     Gets the list of <see cref="IWorkflowCondition" />s that must be true in order to leave this state.
        /// </summary>
        public List<IWorkflowCondition> ExitConditions
        {
            get => this.exitConditions ?? (this.exitConditions = new List<IWorkflowCondition>());

            private set => this.exitConditions = value;
        }

        /// <summary>
        ///     Gets or sets the id for this state.
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        ///     Gets the list of transitions that can be used to leave this state.
        /// </summary>
        public List<WorkflowTransition> Transitions
        {
            get => this.transitions ?? (this.transitions = new List<WorkflowTransition>());

            private set => this.transitions = value;
        }

        /// <summary>
        ///     Creates a new <see cref="WorkflowTransition" /> between this and a target <see cref="WorkflowState" />.
        /// </summary>
        /// <param name="targetState">The target of the transition.</param>
        /// <param name="id">
        ///     The id of the new transition.
        /// </param>
        /// <param name="displayName">
        ///     The display name for the new transition.
        /// </param>
        /// <param name="description">
        ///     A description of the new transition.
        /// </param>
        /// <returns>
        ///     The new <see cref="WorkflowTransition" />.
        /// </returns>
        public WorkflowTransition CreateTransition(
            WorkflowState targetState,
            string id = null,
            string displayName = null,
            string description = null)
        {
            var transition = new WorkflowTransition();
            if (!string.IsNullOrEmpty(id))
            {
                transition.Id = id;
            }

            if (!string.IsNullOrEmpty(displayName))
            {
                transition.DisplayName = displayName;
            }

            if (!string.IsNullOrEmpty(description))
            {
                transition.Description = description;
            }

            transition.TargetStateId = targetState.Id;

            this.Transitions.Add(transition);

            return transition;
        }

        /// <summary>
        ///     Gets the list of interests for this state.
        /// </summary>
        /// <param name="instance">
        ///     The workflow instance that the state belongs to.
        /// </param>
        /// <returns>
        ///     The list of interests for the state.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The list of interests is built up by concatenating all interests from
        ///         - the entry conditions
        ///         - the exit conditions
        ///         - the transitions.
        ///     </para>
        /// </remarks>
        public IEnumerable<string> GetInterests(WorkflowInstance instance)
        {
            IEnumerable<string> entryConditionInterests = this.EntryConditions.SelectMany(c => c.GetInterests(instance));
            IEnumerable<string> exitConditionInterests = this.ExitConditions.SelectMany(c => c.GetInterests(instance));
            IEnumerable<string> transitionInterests = this.Transitions.SelectMany(t => t.GetInterests(instance));

            return entryConditionInterests.Concatenate(exitConditionInterests, transitionInterests).Distinct().ToList();
        }
    }
}