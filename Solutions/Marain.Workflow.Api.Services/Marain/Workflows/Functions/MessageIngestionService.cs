// <copyright file="MessageIngestionService.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Functions
{
    using System;
    using System.Threading.Tasks;

    using Marain.OpenApi;
    using Marain.Operations.Client.OperationsControl;
    using Marain.Operations.Client.OperationsControl.Models;
    using Marain.Telemetry;
    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;

    /// <summary>
    ///     Handles incoming triggers posted to the trigger service.
    /// </summary>
    [EmbeddedOpenApiDefinition("Marain.Workflows.Functions.MessageIngestionService.yaml")]
    public class MessageIngestionService : IOpenApiService
    {
        private const string StartNewWorkflowOperationId = "sendStartNewWorkflowInstanceRequest";
        private const string TriggerWorkflowOperationId = "sendTrigger";

        private readonly IWorkflowMessageQueue messageQueue;
        private readonly TelemetryClient telemetryClient;
        private readonly IEndjinOperationsControl operationsControl;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MessageIngestionService" /> class.
        /// </summary>
        /// <param name="messageQueue">
        ///     The trigger queue to publish new triggers to.
        /// </param>
        /// <param name="telemetryClient">A <see cref="TelemetryClient"/> to log telemetry.</param>
        /// <param name="operationsControl">Allows definition and control of long-running operations.</param>
        public MessageIngestionService(
            IWorkflowMessageQueue messageQueue,
            TelemetryClient telemetryClient,
            IEndjinOperationsControl operationsControl)
        {
            this.messageQueue = messageQueue;
            this.telemetryClient = telemetryClient;
            this.operationsControl = operationsControl;
        }

        /// <summary>
        ///     Accepts a new request and enqueues it.
        /// </summary>
        /// <param name="body">
        ///     The request.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        [OperationId(StartNewWorkflowOperationId)]
        public async Task<OpenApiResult> HandleStartNewWorkflowInstanceRequest(StartWorkflowInstanceRequest body)
        {
            using (this.telemetryClient.StartOperation<RequestTelemetry>(StartNewWorkflowOperationId))
            {
                TelemetryOperationContext.Current.Properties["Endjin_WorkflowRequestId"] = body.RequestId;
                TelemetryOperationContext.Current.Properties["Endjin_WorkflowId"] = body.WorkflowId;

                var operationId = Guid.NewGuid();
                CreateOperationHeaders operationHeaders =
                    await this.operationsControl.CreateOperationAsync(operationId).ConfigureAwait(false);

                await this.messageQueue.EnqueueStartWorkflowInstanceRequestAsync(body, operationId).ConfigureAwait(false);

                return this.AcceptedResult(operationHeaders.Location);
            }
        }

        /// <summary>
        ///     Accepts an incoming trigger and enqueues it.
        /// </summary>
        /// <param name="body">
        ///     The trigger.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        [OperationId(TriggerWorkflowOperationId)]
        public async Task<OpenApiResult> HandleTrigger(IWorkflowTrigger body)
        {
            using (this.telemetryClient.StartOperation<RequestTelemetry>(TriggerWorkflowOperationId))
            {
                TelemetryOperationContext.Current.Properties["Endjin_WorkflowTriggerId"] = body.Id;

                var operationId = Guid.NewGuid();
                CreateOperationHeaders operationHeaders =
                    await this.operationsControl.CreateOperationAsync(operationId).ConfigureAwait(false);

                await this.messageQueue.EnqueueTriggerAsync(body, operationId).ConfigureAwait(false);

                return this.AcceptedResult(operationHeaders.Location);
            }
        }
    }
}