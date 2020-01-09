// <copyright file="MessageIngestionService.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Functions
{
    using System;
    using System.Threading.Tasks;
    using Marain.Operations.Client.OperationsControl;
    using Marain.Operations.Client.OperationsControl.Models;
    using Menes;

    /// <summary>
    /// Handles incoming triggers posted to the trigger service.
    /// </summary>
    [EmbeddedOpenApiDefinition("Marain.Workflows.Functions.MessageIngestionService.yaml")]
    public class MessageIngestionService : IOpenApiService
    {
        private const string StartNewWorkflowOperationId = "sendStartNewWorkflowInstanceRequest";
        private const string TriggerWorkflowOperationId = "sendTrigger";

        private readonly IWorkflowMessageQueue messageQueue;
        private readonly IMarainOperationsControl operationsControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageIngestionService" /> class.
        /// </summary>
        /// <param name="messageQueue">The trigger queue to publish new triggers to.</param>
        /// <param name="operationsControl">Allows definition and control of long-running operations.</param>
        public MessageIngestionService(
            IWorkflowMessageQueue messageQueue,
            IMarainOperationsControl operationsControl)
        {
            this.messageQueue = messageQueue;
            this.operationsControl = operationsControl;
        }

        /// <summary>
        /// Accepts a new request and enqueues it.
        /// </summary>
        /// <param name="tenantId">The tenant Id for the current request.</param>
        /// <param name="body">The request.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [OperationId(StartNewWorkflowOperationId)]
        public async Task<OpenApiResult> HandleStartNewWorkflowInstanceRequest(string tenantId, StartWorkflowInstanceRequest body)
        {
            var operationId = Guid.NewGuid();
            CreateOperationHeaders operationHeaders = await this.operationsControl.CreateOperationAsync(tenantId, operationId).ConfigureAwait(false);

            await this.messageQueue.EnqueueStartWorkflowInstanceRequestAsync(body, operationId).ConfigureAwait(false);

            return this.AcceptedResult(operationHeaders.Location);
        }

        /// <summary>
        /// Accepts an incoming trigger and enqueues it.
        /// </summary>
        /// <param name="tenantId">The tenant Id for the current request.</param>
        /// <param name="body">The request.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [OperationId(TriggerWorkflowOperationId)]
        public async Task<OpenApiResult> HandleTrigger(string tenantId, IWorkflowTrigger body)
        {
            var operationId = Guid.NewGuid();
            CreateOperationHeaders operationHeaders =
                await this.operationsControl.CreateOperationAsync(tenantId, operationId).ConfigureAwait(false);

            await this.messageQueue.EnqueueTriggerAsync(body, operationId).ConfigureAwait(false);

            return this.AcceptedResult(operationHeaders.Location);
        }
    }
}