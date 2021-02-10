// <copyright file="WorkflowInstanceTransitionStartedEvent.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.DomainEvents
{
    /// <summary>
    /// A trigger has been processed by the workflow instance and has resulted in a trigger being executed.
    /// </summary>
    public class WorkflowInstanceTransitionStartedEvent : DomainEvent
    {
        /// <summary>
        /// The registered content type used when this object is serialized/deserialized.
        /// </summary>
        public const string RegisteredContentType = DomainEvent.ContentTypeBase + "workflowinstances.transitionstarted.v1";

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowInstanceCreatedEvent"/> class.
        /// </summary>
        /// <param name="aggregateId">The <see cref="DomainEvent.AggregateId"/>.</param>
        /// <param name="sequenceNumber">The sequence number of the event. Should be monotonically increasing for the aggregate.</param>
        /// <param name="transitionId">The <see cref="TransitionId"/>.</param>
        /// <param name="targetStateId">The <see cref="TargetStateId"/>.</param>
        /// <param name="trigger">The <see cref="Trigger"/>.</param>
        public WorkflowInstanceTransitionStartedEvent(
            string aggregateId,
            int sequenceNumber,
            string transitionId,
            string targetStateId,
            IWorkflowTrigger trigger)
            : base(aggregateId, sequenceNumber)
        {
            this.TransitionId = transitionId;
            this.TargetStateId = targetStateId;
            this.Trigger = trigger;
        }

        /// <inheritdoc/>
        public override string ContentType => RegisteredContentType;

        /// <summary>
        /// Gets the Id of the transition which has started.
        /// </summary>
        public string TransitionId { get; }

        /// <summary>
        /// Gets the Id of the state which this transition is to.
        /// </summary>
        public string TargetStateId { get; }

        /// <summary>
        /// Gets the trigger which is causing the transition.
        /// </summary>
        public IWorkflowTrigger Trigger { get; }
    }
}
