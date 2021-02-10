// <copyright file="WorkflowInstanceActiveTransitionState.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Internal
{
    using Marain.Workflows.DomainEvents;

    /// <summary>
    /// Data related to an active transition taking place for a <see cref="WorkflowInstance"/>.
    /// </summary>
    public class WorkflowInstanceActiveTransitionState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowInstanceActiveTransitionState"/> class.
        /// </summary>
        /// <param name="transitionStartedEventVersion">The <see cref="TransitionStartedEventVersion"/>.</param>
        /// <param name="transitionId">The <see cref="TransitionId"/>.</param>
        /// <param name="targetStateId">The <see cref="TargetStateId"/>.</param>
        /// <param name="trigger">The <see cref="Trigger"/>.</param>
        /// <param name="previousStateId">The <see cref="PreviousStateId"/>.</param>
        public WorkflowInstanceActiveTransitionState(
            long transitionStartedEventVersion,
            string transitionId,
            string targetStateId,
            IWorkflowTrigger trigger,
            string previousStateId)
        {
            this.TransitionStartedEventVersion = transitionStartedEventVersion;
            this.TransitionId = transitionId;
            this.TargetStateId = targetStateId;
            this.Trigger = trigger;
            this.PreviousStateId = previousStateId;
        }

        /// <summary>
        /// Gets the sequence number of the <see cref="WorkflowInstanceTransitionStartedEvent"/> that
        /// began the active transition.
        /// </summary>
        public long TransitionStartedEventVersion { get; }

        /// <summary>
        /// Gets the <see cref="WorkflowTransition.Id"/> of the transition that is executing.
        /// </summary>
        public string TransitionId { get; }

        /// <summary>
        /// Gets the target state for the transition.
        /// </summary>
        public string TargetStateId { get; }

        /// <summary>
        /// Gets the <see cref="IWorkflowTrigger"/> that initiated this transition.
        /// </summary>
        public IWorkflowTrigger Trigger { get; }

        /// <summary>
        /// Gets the Id of the state that the instance was in when this transition started.
        /// </summary>
        public string PreviousStateId { get; }
    }
}
