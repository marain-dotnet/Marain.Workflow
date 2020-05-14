// <copyright file="InMemoryWorkflowMessageQueue.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Specs.TestObjects
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Corvus.Json;
    using Corvus.Tenancy;
    using Microsoft.Extensions.Logging;
    using NUnit.Framework.Internal;

    /// <inheritdoc />
    /// <summary>
    /// An in memory implementation of the <see cref="IWorkflowMessageQueue" />
    /// interface that accepts new triggers and hands them off to the
    /// appropriate <see cref="IWorkflowEngine" />.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     As it currently stands, this implementation is not suitable for use
    ///     in any kind of production scenario. Any errors thrown out of the workflow
    ///     engine would cause the background processing thread to end, preventing
    ///     any further messages from being processed.
    /// </para>
    /// </remarks>
    public class InMemoryWorkflowMessageQueue : IWorkflowMessageQueue
    {
        /// <summary>
        /// The logger that will be used for diagnostic messages.
        /// </summary>
        private readonly ILogger<InMemoryWorkflowMessageQueue> logger;

        /// <summary>
        /// The queue that will be used for triggers.
        /// </summary>
        private readonly ConcurrentQueue<WorkflowMessageEnvelope> queue;

        /// <summary>
        /// The factory for the workflow engine to which triggers will be passed.
        /// </summary>
        private readonly ITenantedWorkflowEngineFactory workflowEngineFactory;

        /// <summary>
        /// The factory for the workflow instance store.
        /// </summary>
        private readonly ITenantedWorkflowInstanceStoreFactory workflowInstanceStoreFactory;

        /// <summary>
        /// The <see cref="IPropertyBagFactory"/> that will be used when creating new
        /// <see cref="WorkflowMessageEnvelope"/> instances.
        /// </summary>
        private readonly IPropertyBagFactory propertyBagFactory;

        /// <summary>
        /// The task that represents the procssing thread.
        /// </summary>
        private Task runner;
        private ITenant tenant;

        /// <summary>
        /// Flag that indicates whether or not the <see cref="FinishProcessing" /> method has
        /// been called to tall the processing thread to finish processing queued triggers
        /// but not accept any new ones.
        /// </summary>
        private bool shouldComplete;

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryWorkflowMessageQueue" /> class.
        /// </summary>
        /// <param name="workflowEngineFactory">
        /// The workflow engine factory to create the engine to which to hand off the triggers.
        /// </param>
        /// <param name="workflowInstanceStoreFactory">
        /// The workflow instance store factory to use to access underlying instance storage.
        /// </param>
        /// <param name="propertyBagFactory">
        /// The <see cref="IPropertyBagFactory"/> that will be used when creating new
        /// <see cref="WorkflowMessageEnvelope"/> instances.
        /// </param>
        /// <param name="logger">
        /// Logger to use to write diagnostic messages.
        /// </param>
        /// <remarks>
        /// <para>
        ///     The queue will be created in the "Stopped" state. To begin processing,
        ///     call the <see cref="StartProcessing" /> method.
        /// </para>
        /// </remarks>
        public InMemoryWorkflowMessageQueue(
            ITenantedWorkflowEngineFactory workflowEngineFactory,
            ITenantedWorkflowInstanceStoreFactory workflowInstanceStoreFactory,
            IPropertyBagFactory propertyBagFactory,
            ILogger<InMemoryWorkflowMessageQueue> logger)
        {
            this.logger = logger;
            this.workflowEngineFactory = workflowEngineFactory;
            this.queue = new ConcurrentQueue<WorkflowMessageEnvelope>();
            this.workflowInstanceStoreFactory = workflowInstanceStoreFactory;
            this.propertyBagFactory = propertyBagFactory;
        }

        /// <inheritdoc />
        public Task EnqueueStartWorkflowInstanceRequestAsync(
            StartWorkflowInstanceRequest request,
            Guid operationId)
        {
            return this.EnqueueMessageEnvelopeAsync(
                new WorkflowMessageEnvelope(this.propertyBagFactory.Create(PropertyBagValues.Empty))
                {
                    StartWorkflowInstanceRequest = request,
                });
        }

        /// <inheritdoc />
        /// <exception cref="InvalidOperationException">
        /// Thrown if the <see cref="FinishProcessing" /> method has been called.
        /// </exception>
        public Task EnqueueTriggerAsync(
            IWorkflowTrigger trigger,
            Guid operationId)
        {
            return this.EnqueueMessageEnvelopeAsync(
                new WorkflowMessageEnvelope(this.propertyBagFactory.Create(PropertyBagValues.Empty))
                {
                    Trigger = trigger,
                });
        }

        /// <summary>
        /// Signals the <see cref="Process" /> task that it should finish processing currently queued
        /// triggers and then stop.
        /// </summary>
        /// <returns>
        /// A <see cref="Task" /> that will finish when all triggers in the queue have been processed.
        /// </returns>
        public Task FinishProcessing()
        {
            this.shouldComplete = true;
            return this.runner;
        }

        /// <summary>
        /// Start passing trigger messsages to the <see cref="IWorkflowEngine" /> that was
        /// passed to the constructor.
        /// </summary>
        /// <param name="tenant">The tenant for which to start processing.</param>
        /// <exception cref="InvalidOperationException">
        /// Thrown if processing is already started.
        /// </exception>
        public void StartProcessing(ITenant tenant)
        {
            if (this.runner != null && this.runner.Status != TaskStatus.RanToCompletion
                                    && this.runner.Status != TaskStatus.Faulted)
            {
                throw new InvalidOperationException();
            }

            this.tenant = tenant;
            this.shouldComplete = false;
            this.runner = new Task(() => this.Process().Wait());
            this.runner.Start();
        }

        private Task EnqueueMessageEnvelopeAsync(WorkflowMessageEnvelope envelope)
        {
            if (this.shouldComplete)
            {
                throw new InvalidOperationException();
            }

            this.queue.Enqueue(envelope);

            return Task.CompletedTask;
        }

        /// <summary>
        /// The Process method is invoked asynchronously when <see cref="StartProcessing" /> is
        /// called. It will run on a background thread until <see cref="FinishProcessing" />
        /// is called.
        /// </summary>
        /// <returns>
        /// A <see cref="Task" /> that will complete after <see cref="FinishProcessing" /> is called.
        /// </returns>
        private async Task Process()
        {
            while (true)
            {
                if (this.queue.IsEmpty)
                {
                    if (this.shouldComplete)
                    {
                        break;
                    }

                    await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
                    continue;
                }

                this.queue.TryPeek(out WorkflowMessageEnvelope item);

                IWorkflowInstanceStore instanceStore =
                    await this.workflowInstanceStoreFactory.GetWorkflowInstanceStoreForTenantAsync(this.tenant).ConfigureAwait(false);
                IWorkflowEngine engine =
                    await this.workflowEngineFactory.GetWorkflowEngineAsync(this.tenant).ConfigureAwait(false);

                if (item.IsTrigger)
                {
                    this.logger.LogInformation("Processing trigger with content type " + item.ContentType);

                    IWorkflowTrigger trigger = item.Trigger;

                    IEnumerable<string> instanceIds = await instanceStore.GetMatchingWorkflowInstanceIdsForSubjectsAsync(
                                          item.Trigger.GetSubjects(),
                                          int.MaxValue,
                                          0).ConfigureAwait(false);

                    foreach (string current in instanceIds)
                    {
                        await engine.ProcessTriggerAsync(trigger, current).ConfigureAwait(false);
                    }
                }
                else
                {
                    this.logger.LogInformation("Processing start workflow instance request");
                    await engine.StartWorkflowInstanceAsync(item.StartWorkflowInstanceRequest)
                        .ConfigureAwait(false);
                }

                this.queue.TryDequeue(out _);
            }
        }
    }
}