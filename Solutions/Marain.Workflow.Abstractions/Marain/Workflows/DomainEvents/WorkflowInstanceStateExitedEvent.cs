// <copyright file="WorkflowInstanceStateExitedEvent.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.DomainEvents
{
    using System.Collections.Generic;
    using System.Collections.Immutable;

    /// <summary>
    /// A trigger has been processed by the workflow instance and has resulted in a trigger being executed.
    /// </summary>
    public class WorkflowInstanceStateExitedEvent : DomainEvent
    {
        /// <summary>
        /// The registered content type used when this object is serialized/deserialized.
        /// </summary>
        public const string RegisteredContentType = DomainEvent.ContentTypeBase + "workflowinstances.stateexited.v1";

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowInstanceCreatedEvent"/> class.
        /// </summary>
        /// <param name="workflowInstanceId">The <see cref="DomainEvent.AggregateId"/>.</param>
        /// <param name="sequenceNumber">The sequence number of the event. Should be monotonically increasing for the aggregate.</param>
        /// <param name="transitionStartedSequenceNumber">The <see cref="TransitionStartedSequenceNumber"/>.</param>
        /// <param name="exitedStateId">The <see cref="ExitedStateId"/>.</param>
        /// <param name="addedAndUpdatedContextItems">The <see cref="AddedAndUpdatedContextItems"/>.</param>
        /// <param name="removedContextItems">The <see cref="RemovedContextItems"/>.</param>
        public WorkflowInstanceStateExitedEvent(
            string workflowInstanceId,
            long sequenceNumber,
            long transitionStartedSequenceNumber,
            string exitedStateId,
            IEnumerable<KeyValuePair<string, string>> addedAndUpdatedContextItems,
            IEnumerable<string> removedContextItems)
            : base(workflowInstanceId, sequenceNumber)
        {
            // TODO: Null checks.
            this.ExitedStateId = exitedStateId;
            this.TransitionStartedSequenceNumber = transitionStartedSequenceNumber;
            this.AddedAndUpdatedContextItems = addedAndUpdatedContextItems.ToImmutableDictionary();
            this.RemovedContextItems = removedContextItems.ToImmutableArray();
        }

        /// <inheritdoc/>
        public override string ContentType => RegisteredContentType;

        /// <summary>
        /// Gets the Id of the state that's been exited.
        /// </summary>
        public string ExitedStateId { get; }

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
