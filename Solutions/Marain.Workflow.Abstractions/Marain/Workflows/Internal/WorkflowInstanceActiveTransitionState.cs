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
        /// Gets or sets the sequence number of the <see cref="WorkflowInstanceTransitionStartedEvent"/> that
        /// began the active transition.
        /// </summary>
        public long TransitionStartedEventVersion { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="WorkflowTransition.Id"/> of the transition that is executing.
        /// </summary>
        public string TransitionId { get; set; }

        /// <summary>
        /// Gets or sets the target state for the transition.
        /// </summary>
        public string TargetStateId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IWorkflowTrigger"/> that initiated this transition.
        /// </summary>
        public IWorkflowTrigger Trigger { get; set; }

        /// <summary>
        /// Gets or sets the Id of the state that the instance was in when this transition started.
        /// </summary>
        public string InitialStateId { get; set; }
    }
}
