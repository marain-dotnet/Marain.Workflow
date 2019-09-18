// <copyright file="IWorkflowMessageQueue.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    ///     Interface for a service which enqueues messages to be processed
    ///     by the workflow engine.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         The queue must operate in a "fire and forget" fashion - i.e. as soon
    ///         as a message has been enqueued, further processing must take place
    ///         asynchronously (see notes on <see cref="EnqueueTriggerAsync" /> for more
    ///         details.
    ///     </para>
    ///     <para>
    ///         An implementation of this interface can also handle passing the messages
    ///         to the workflow engine (as with the InMemoryWorkflowMessageQueue)
    ///         but this is not mandatory. A full implementation of the workflow engine
    ///         will rely on other means to dequeue messages and pass them into the engine.
    ///     </para>
    /// </remarks>
    public interface IWorkflowMessageQueue
    {
        /// <summary>
        ///     Enqueues a new trigger for eventual processing by the
        ///     <see cref="IWorkflowEngine" />.
        /// </summary>
        /// <param name="trigger">
        ///     The trigger to enqueue.
        /// </param>
        /// <param name="operationId">
        ///     The unique id of the operation being used to track this work.
        /// </param>
        /// <returns>
        ///     A <see cref="Task" /> that completes when the trigger has been
        ///     successfully enqueued.
        /// </returns>
        Task EnqueueTriggerAsync(
            IWorkflowTrigger trigger,
            Guid operationId);

        /// <summary>
        ///     Enqueues a new instance creation requestfor eventual processing
        ///     by the <see cref="IWorkflowEngine" />.
        /// </summary>
        /// <param name="request">
        ///     The request to enqueue.
        /// </param>
        /// <param name="operationId">
        ///     The unique id of the operation being used to track this work.
        /// </param>
        /// <returns>
        ///     A <see cref="Task" /> that completes when the request has been
        ///     successfully enqueued.
        /// </returns>
        Task EnqueueStartWorkflowInstanceRequestAsync(
            StartWorkflowInstanceRequest request,
            Guid operationId);
    }
}