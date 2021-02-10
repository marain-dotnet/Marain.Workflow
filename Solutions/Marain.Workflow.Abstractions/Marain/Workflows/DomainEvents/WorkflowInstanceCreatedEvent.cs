// <copyright file="WorkflowInstanceCreatedEvent.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.DomainEvents
{
    using System.Collections.Generic;
    using System.Collections.Immutable;

    /// <summary>
    /// Workflow instance has been created.
    /// </summary>
    public class WorkflowInstanceCreatedEvent : DomainEvent
    {
        /// <summary>
        /// The registered content type used when this object is serialized/deserialized.
        /// </summary>
        public const string RegisteredContentType = DomainEvent.ContentTypeBase + "workflowinstances.created.v1";

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowInstanceCreatedEvent"/> class.
        /// </summary>
        /// <param name="aggregateId">The <see cref="DomainEvent.AggregateId"/>.</param>
        /// <param name="sequenceNumber">The sequence number of the event. Should be monotonically increasing for the aggregate.</param>
        /// <param name="workflowId">The <see cref="WorkflowId"/>.</param>
        /// <param name="initialStateId">The <see cref="InitialStateId"/>.</param>
        /// <param name="context">The <see cref="Context"/>.</param>
        public WorkflowInstanceCreatedEvent(
            string aggregateId,
            int sequenceNumber,
            string workflowId,
            string initialStateId,
            IImmutableDictionary<string, string> context)
            : base(aggregateId, sequenceNumber)
        {
            // TODO: Null checks
            this.WorkflowId = workflowId;
            this.InitialStateId = initialStateId;
            this.Context = context;
        }

        /// <inheritdoc/>
        public override string ContentType => RegisteredContentType;

        /// <summary>
        /// Gets the Id of the workflow to which this instance belongs.
        /// </summary>
        public string WorkflowId { get; }

        /// <summary>
        /// Gets the Id of the state that the instance should be in once initialization is complete.
        /// </summary>
        public string InitialStateId { get; }

        /// <summary>
        /// Gets the context dictionary for the new instance.
        /// </summary>
        public IImmutableDictionary<string, string> Context { get; }
    }
}
