﻿// <copyright file="WorkflowInstanceStateEnteredEvent.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.DomainEvents
{
    using System.Collections.Generic;
    using System.Collections.Immutable;

    /// <summary>
    /// A trigger has been processed by the workflow instance and has resulted in a trigger being executed.
    /// </summary>
    public class WorkflowInstanceStateEnteredEvent : DomainEvent
    {
        /// <summary>
        /// The registered content type used when this object is serialized/deserialized.
        /// </summary>
        public const string RegisteredContentType = DomainEvent.ContentTypeBase + "workflowinstances.stateentered.v1";

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowInstanceCreatedEvent"/> class.
        /// </summary>
        /// <param name="workflowInstanceId">The <see cref="DomainEvent.AggregateId"/>.</param>
        /// <param name="sequenceNumber">The sequence number of the event. Should be monotonically increasing for the aggregate.</param>
        /// <param name="transitionStartedSequenceNumber">The <see cref="TransitionStartedSequenceNumber"/>.</param>
        /// <param name="enteredStateId">The <see cref="EnteredStateId"/>.</param>
        /// <param name="addedAndUpdatedContextItems">The <see cref="AddedAndUpdatedContextItems"/>.</param>
        /// <param name="removedContextItems">The <see cref="RemovedContextItems"/>.</param>
        /// <param name="interests">The <see cref="Interests"/>.</param>
        public WorkflowInstanceStateEnteredEvent(
            string workflowInstanceId,
            long sequenceNumber,
            long transitionStartedSequenceNumber,
            string enteredStateId,
            IEnumerable<KeyValuePair<string, string>> addedAndUpdatedContextItems,
            IEnumerable<string> removedContextItems,
            IEnumerable<string> interests)
            : base(workflowInstanceId, sequenceNumber)
        {
            // TODO: Null checks.
            this.EnteredStateId = enteredStateId;
            this.TransitionStartedSequenceNumber = transitionStartedSequenceNumber;
            this.AddedAndUpdatedContextItems = addedAndUpdatedContextItems.ToImmutableDictionary();
            this.RemovedContextItems = removedContextItems.ToImmutableArray();
            this.Interests = interests.ToImmutableArray();
        }

        /// <inheritdoc/>
        public override string ContentType => RegisteredContentType;

        /// <summary>
        /// Gets the Id of the state that's been exited.
        /// </summary>
        public string EnteredStateId { get; }

        /// <summary>
        /// Gets the version number of the first event in the transition that resulted in us entering this state. If
        /// this event resulted from initializing the workflow instance, the value will be 0.
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

        /// <summary>
        /// Gets the new interests for the workflow instance, based on its current state.
        /// </summary>
        public IImmutableList<string> Interests { get; }
    }
}