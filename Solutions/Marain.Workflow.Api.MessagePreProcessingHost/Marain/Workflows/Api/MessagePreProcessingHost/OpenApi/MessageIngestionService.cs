// <copyright file="MessageIngestionService.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.MessagePreProcessingHost.OpenApi
{
    using System;
    using System.Threading.Tasks;
    using Corvus.Extensions.Json;
    using Marain.Operations.Client.OperationsControl;
    using Marain.Operations.Client.OperationsControl.Models;
    using Marain.Workflows.Api.MessagePreProcessingHost.Orchestrators;
    using Marain.Workflows.Api.MessagePreProcessingHost.Shared;
    using Menes;

    /// <summary>
    /// Handles incoming triggers posted to the trigger service.
    /// </summary>
    [EmbeddedOpenApiDefinition("Marain.Workflows.Api.MessagePreProcessingHost.OpenApi.MessageIngestionService.yaml")]
    public class MessageIngestionService : IOpenApiService
    {
        private const string StartNewWorkflowOperationId = "sendStartNewWorkflowInstanceRequest";
        private const string TriggerWorkflowOperationId = "sendTrigger";

        private readonly IMarainOperationsControl operationsControl;
        private readonly IJsonSerializerSettingsProvider serializerSettingsProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageIngestionService" /> class.
        /// </summary>
        /// <param name="serializerSettingsProvider">Serialization settings provider.</param>
        /// <param name="operationsControl">Allows definition and control of long-running operations.</param>
        public MessageIngestionService(
            IJsonSerializerSettingsProvider serializerSettingsProvider,
            IMarainOperationsControl operationsControl)
        {
            this.operationsControl = operationsControl;
            this.serializerSettingsProvider = serializerSettingsProvider;
        }

        /// <summary>
        /// Accepts a new request and enqueues it.
        /// </summary>
        /// <param name="context">The current OpenApi context.</param>
        /// <param name="body">The request.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [OperationId(StartNewWorkflowOperationId)]
        public async Task<OpenApiResult> HandleStartNewWorkflowInstanceRequest(IOpenApiContext context, StartWorkflowInstanceRequest body)
        {
            var operationId = Guid.NewGuid();
            CreateOperationHeaders operationHeaders = await this.operationsControl.CreateOperationAsync(context.CurrentTenantId, operationId).ConfigureAwait(false);

            var envelope = new WorkflowMessageEnvelope
            {
                StartWorkflowInstanceRequest = body,
                OperationId = operationId,
                TenantId = context.CurrentTenantId,
            };

            var durableFunctionsOpenApiContext = (DurableFunctionsOpenApiContext)context;

            await durableFunctionsOpenApiContext.OrchestrationClient.StartNewWithCustomSerializationSettingsAsync(
                     nameof(TriggerExecutionOrchestrator),
                     operationId.ToString(),
                     envelope,
                     this.serializerSettingsProvider.Instance).ConfigureAwait(false);

            return this.AcceptedResult(operationHeaders.Location);
        }

        /// <summary>
        /// Accepts an incoming trigger and enqueues it.
        /// </summary>
        /// <param name="context">The current OpenApi context.</param>
        /// <param name="body">The request.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [OperationId(TriggerWorkflowOperationId)]
        public async Task<OpenApiResult> HandleTrigger(IOpenApiContext context, IWorkflowTrigger body)
        {
            var operationId = Guid.NewGuid();
            CreateOperationHeaders operationHeaders =
                await this.operationsControl.CreateOperationAsync(context.CurrentTenantId, operationId).ConfigureAwait(false);

            var envelope = new WorkflowMessageEnvelope
            {
                Trigger = body,
                OperationId = operationId,
                TenantId = context.CurrentTenantId,
            };

            var durableFunctionsOpenApiContext = (DurableFunctionsOpenApiContext)context;

            await durableFunctionsOpenApiContext.OrchestrationClient.StartNewWithCustomSerializationSettingsAsync(
                     nameof(TriggerExecutionOrchestrator),
                     operationId.ToString(),
                     envelope,
                     this.serializerSettingsProvider.Instance).ConfigureAwait(false);

            return this.AcceptedResult(operationHeaders.Location);
        }
    }
}