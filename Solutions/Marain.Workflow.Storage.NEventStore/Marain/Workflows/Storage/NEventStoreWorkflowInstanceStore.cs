// <copyright file="NEventStoreWorkflowInstanceStore.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Storage
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Threading.Tasks;
    using Marain.Workflows.DomainEvents;
    using Microsoft.Extensions.Logging;
    using NEventStore;

    /// <summary>
    /// NEventStore based implementation of <see cref="IWorkflowInstanceStore"/>.
    /// </summary>
    public class NEventStoreWorkflowInstanceStore : IWorkflowInstanceStore
    {
        private readonly IStoreEvents store;
        private readonly ILogger<NEventStoreWorkflowInstanceStore> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NEventStoreWorkflowInstanceStore"/> class.
        /// </summary>
        /// <param name="store">The underlying event store.</param>
        /// <param name="logger">The logger.</param>
        public NEventStoreWorkflowInstanceStore(
            IStoreEvents store,
            ILogger<NEventStoreWorkflowInstanceStore> logger)
        {
            this.store = store
                ?? throw new ArgumentNullException(nameof(store));

            this.logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public Task DeleteWorkflowInstanceAsync(string workflowInstanceId, string? partitionKey = null)
        {
            if (string.IsNullOrEmpty(workflowInstanceId))
            {
                throw new ArgumentNullException(nameof(workflowInstanceId));
            }

            this.store.Advanced.DeleteStream(
                partitionKey ?? workflowInstanceId,
                workflowInstanceId);

            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task<WorkflowInstance> GetWorkflowInstanceAsync(string workflowInstanceId, string? partitionKey = null)
        {
            if (string.IsNullOrEmpty(workflowInstanceId))
            {
                throw new ArgumentNullException(nameof(workflowInstanceId));
            }

            // TODO: Integrate snapshotting
            IEventStream stream = this.store.OpenStream(
                partitionKey ?? workflowInstanceId,
                workflowInstanceId,
                int.MinValue,
                int.MaxValue);

            // Map the committed events to something we recognise.
            IEnumerable<DomainEvent> domainEvents = stream.CommittedEvents.Select(ev => (DomainEvent)ev.Body);

            var instance = WorkflowInstance.FromCommittedEvents(domainEvents);

            return Task.FromResult(instance);
        }

        /// <inheritdoc/>
        public Task UpsertWorkflowInstanceAsync(WorkflowInstance workflowInstance, string? partitionKey = null)
        {
            IImmutableList<DomainEvent>? eventsToStore = workflowInstance.GetUncommittedEvents();

            long firstSequenceNumber = eventsToStore[0].SequenceNumber;

            // If the first event in that list has Sequence 1, then we need to create a stream for the instance.
            // Otherwise we want to open the stream.
            IEventStream stream = firstSequenceNumber == 1
                ? this.store.CreateStream(partitionKey ?? workflowInstance.Id, workflowInstance.Id)
                : this.store.OpenStream(partitionKey ?? workflowInstance.Id, workflowInstance.Id, int.MinValue, int.MaxValue);

            if (stream.StreamRevision != firstSequenceNumber - 1)
            {
                throw new InvalidOperationException($"The supplied WorkflowInstance, '{workflowInstance.Id}' has uncommitted events whose first sequence number, {firstSequenceNumber}, does not directly follow on from the last committed event, {stream.StreamRevision}.");
            }

            foreach (DomainEvent ev in eventsToStore)
            {
                stream.Add(new EventMessage { Body = ev });
            }

            var commitId = Guid.NewGuid();

            this.logger.LogDebug(
                "About to commit {eventCount} events for workflow instance '{workflowInstanceId}' with commit Id '{commitId}'. Event range is from {fromSequenceNumber} to {toSequenceNumber}",
                eventsToStore.Count,
                workflowInstance.Id,
                commitId,
                eventsToStore[0].SequenceNumber,
                eventsToStore[^1].SequenceNumber);

            stream.CommitChanges(commitId);

            workflowInstance.ClearUncommittedEvents();

            return Task.CompletedTask;
        }
    }
}
