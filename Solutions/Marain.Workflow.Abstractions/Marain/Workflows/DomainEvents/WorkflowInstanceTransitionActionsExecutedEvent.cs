// <copyright file="WorkflowInstanceTransitionActionsExecutedEvent.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.DomainEvents
{
    using System.Collections.Generic;
    using System.Collections.Immutable;

    /// <summary>
    /// Workflow instance has processed any actions directly associated with a transition. This does not signify that
    /// the transition has completed; it will be followed up by a <see cref="WorkflowInstanceStateEnteredEvent"/>
    /// which will signify the end of the transition.
    /// </summary>
    public class WorkflowInstanceTransitionActionsExecutedEvent : DomainEvent
    {
        /// <summary>
        /// The registered content type used when this object is serialized/deserialized.
        /// </summary>
        public const string RegisteredContentType = DomainEvent.ContentTypeBase + "workflowinstances.transitionexecuted.v1";

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowInstanceTransitionActionsExecutedEvent"/> class.
        /// </summary>
        /// <param name="workflowInstanceId">The <see cref="DomainEvent.AggregateId"/>.</param>
        /// <param name="sequenceNumber">The sequence number of the event. Should be monotonically increasing for the aggregate.</param>
        /// <param name="transitionStartedSequenceNumber">The <see cref="TransitionStartedSequenceNumber"/>.</param>
        /// <param name="transitionId">The <see cref="TransitionId"/>.</param>
        /// <param name="addedAndUpdatedContextItems">The <see cref="AddedAndUpdatedContextItems"/>.</param>
        /// <param name="removedContextItems">The <see cref="RemovedContextItems"/>.</param>
        public WorkflowInstanceTransitionActionsExecutedEvent(
            string workflowInstanceId,
            long sequenceNumber,
            long transitionStartedSequenceNumber,
            string transitionId,
            IEnumerable<KeyValuePair<string, string>> addedAndUpdatedContextItems,
            IEnumerable<string> removedContextItems)
            : base(workflowInstanceId, sequenceNumber)
        {
            // TODO: Null checks.
            this.TransitionId = transitionId;
            this.TransitionStartedSequenceNumber = transitionStartedSequenceNumber;
            this.AddedAndUpdatedContextItems = addedAndUpdatedContextItems.ToImmutableDictionary();
            this.RemovedContextItems = removedContextItems.ToImmutableArray();
        }

        /// <inheritdoc/>
        public override string ContentType => RegisteredContentType;

        /// <summary>
        /// Gets the Id of the transition which has started.
        /// </summary>
        public string TransitionId { get; }

        /// <summary>
        /// Gets the version number of the first event in the transition that resulted in us exiting the state.
        /// </summary>
        public long TransitionStartedSequenceNumber { get; }

        /// <summary>
        /// Gets the list of context items that were added/updated as a result of exiting the state.
        /// </summary>
        public IImmutableDictionary<string, string> AddedAndUpdatedContextItems { get; }

        /// <summary>
        /// Gets the list of context items that were removed as a result of exiting the state.
        /// </summary>
        public IImmutableList<string> RemovedContextItems { get; }
    }
}
