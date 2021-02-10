// <copyright file="DomainEvent.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.DomainEvents
{
    /// <summary>
    /// Base class for events specific to the <see cref="WorkflowInstance"/> aggregate.
    /// </summary>
    public abstract class DomainEvent
    {
        /// <summary>
        /// The base content type that inheriting classes should use when defining their own content types.
        /// </summary>
        public const string ContentTypeBase = "application/vnd.marain.workflows.domainevents.";

        /// <summary>
        /// Initializes a new instance of the <see cref="DomainEvent"/> class.
        /// </summary>
        /// <param name="aggregateId">The <see cref="AggregateId"/>.</param>
        /// <param name="sequenceNumber">The <see cref="SequenceNumber"/>.</param>
        protected DomainEvent(string aggregateId, int sequenceNumber)
        {
            this.AggregateId = aggregateId;
            this.SequenceNumber = sequenceNumber;
        }

        /// <summary>
        /// Gets the Id of the workflow instance that create this event.
        /// </summary>
        public string AggregateId { get; }

        /// <summary>
        /// Gets the sequence number of the event.
        /// </summary>
        public int SequenceNumber { get; }

        /// <summary>
        /// Gets the type of the event. This will be a well-known type defined by the <see cref="WorkflowInstance"/>.
        /// </summary>
        public abstract string ContentType { get; }
    }
}
