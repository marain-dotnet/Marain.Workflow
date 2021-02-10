// <copyright file="ProcessEventBatchActivity.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflow.Api.EventStreamProcessor.Activities
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Corvus.Tenancy;
    using global::Marain.Workflows;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.DurableTask;
    using NEventStore;

    /// <summary>
    /// Durable activity function which processes a batch of events for the specified tenant.
    /// </summary>
    public class ProcessEventBatchActivity
    {
        private const int MaxEventsPerBatch = 100;

        private readonly ITenantedNEventStoreFactory eventStoreFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessEventBatchActivity"/> class.
        /// </summary>
        /// <param name="eventStoreFactory">The current tenant provider.</param>
        public ProcessEventBatchActivity(
            ITenantedNEventStoreFactory eventStoreFactory)
        {
            this.eventStoreFactory = eventStoreFactory
                ?? throw new ArgumentNullException(nameof(eventStoreFactory));
        }

        /// <summary>
        /// Executes the durable activity.
        /// </summary>
        /// <param name="context">The <see cref="IDurableActivityContext" />.</param>
        /// <param name="tenant">The tenant to process events for.</param>
        /// <returns>A list of tenant Ids.</returns>
        [FunctionName(nameof(ProcessEventBatchActivity))]
        public async Task<int> RunAction(
            [ActivityTrigger] IDurableActivityContext context,
            ITenant tenant)
        {
            IStoreEvents store = await this.eventStoreFactory.GetStoreForTenantAsync(tenant).ConfigureAwait(false);

            // TODO: Current checkpoint tracking.
            const long currentCheckpoint = 0;
            IEnumerable<ICommit> commits = store.Advanced.GetFrom(currentCheckpoint);

            int processedEvents = 0;

            foreach (ICommit currentCommit in commits)
            {
                foreach (EventMessage currentEvent in currentCommit.Events)
                {
                    await this.ProcessEvent(currentEvent).ConfigureAwait(false);
                }

                await this.UpdateCheckpoint(currentCommit.CheckpointToken).ConfigureAwait(false);
                processedEvents += currentCommit.Events.Count;

                if (processedEvents >= MaxEventsPerBatch)
                {
                    // If we've processed more than what we deem to be "one batch" of events, we're going to stop.
                    // This is to avoid the possibility of exceeding the Activity function's maximum runtime.
                    // TODO: Turn this off when running in a premium plan/app service?
                    break;
                }
            }

            return processedEvents;
        }

        private Task ProcessEvent(EventMessage message)
        {
            return Task.CompletedTask;
        }

        private Task UpdateCheckpoint(long newCheckpointToken)
        {
            return Task.CompletedTask;
        }
    }
}
