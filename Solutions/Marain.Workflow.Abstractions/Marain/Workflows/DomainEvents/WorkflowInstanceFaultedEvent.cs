// <copyright file="WorkflowInstanceFaultedEvent.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.DomainEvents
{
    using System.Collections.Generic;
    using System.Collections.Immutable;

    /// <summary>
    /// Workflow instance has failed to correctly process a transition.
    /// </summary>
    public class WorkflowInstanceFaultedEvent : DomainEvent
    {
        /// <summary>
        /// The registered content type used when this object is serialized/deserialized.
        /// </summary>
        public const string RegisteredContentType = DomainEvent.ContentTypeBase + "workflowinstances.transitionfailed.v1";

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowInstanceCreatedEvent"/> class.
        /// </summary>
        /// <param name="workflowInstanceId">The <see cref="DomainEvent.AggregateId"/>.</param>
        /// <param name="sequenceNumber">The sequence number of the event. Should be monotonically increasing for the aggregate.</param>
        /// <param name="errorMessage">The <see cref="ErrorMessage"/>.</param>
        /// <param name="data">The <see cref="Data"/>.</param>
        public WorkflowInstanceFaultedEvent(string workflowInstanceId, long sequenceNumber, string errorMessage, IDictionary<string, string> data)
            : base(workflowInstanceId, sequenceNumber)
        {
            this.ErrorMessage = errorMessage;
            this.Data = data.ToImmutableDictionary();
        }

        /// <inheritdoc/>
        public override string ContentType => RegisteredContentType;

        /// <summary>
        /// Gets a message describing the high-level cause of the fault.
        /// </summary>
        public string ErrorMessage { get; }

        /// <summary>
        /// Gets a list of additional data associated with the fault.
        /// </summary>
        public IImmutableDictionary<string, string> Data { get; }
    }
}
