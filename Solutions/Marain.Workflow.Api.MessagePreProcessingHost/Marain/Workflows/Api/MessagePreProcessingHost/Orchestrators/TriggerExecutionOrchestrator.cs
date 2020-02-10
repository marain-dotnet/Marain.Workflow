// <copyright file="TriggerExecutionOrchestrator.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

#pragma warning disable RCS1090 // Call 'ConfigureAwait(false)'
namespace Marain.Workflows.Api.MessagePreProcessingHost.Orchestrators
{
    using System;
    using System.Threading.Tasks;
    using Corvus.Extensions.Json;
    using Marain.Workflows.Api.MessagePreProcessingHost.Activities;
    using Marain.Workflows.Api.MessagePreProcessingHost.Shared;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The trigger execution orchestrator.
    /// </summary>
    public class TriggerExecutionOrchestrator
    {
        private readonly IJsonSerializerSettingsProvider serializerSettingsProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerExecutionOrchestrator"/> class.
        /// </summary>
        /// <param name="serializerSettingsProvider">The serialization settings provider.</param>
        public TriggerExecutionOrchestrator(IJsonSerializerSettingsProvider serializerSettingsProvider)
        {
            this.serializerSettingsProvider = serializerSettingsProvider;
        }

        /// <summary>
        /// The run orchestrator.
        /// </summary>
        /// <param name="orchestrationContext">
        /// The context.
        /// </param>
        /// <param name="executionContext">
        /// The execution context.
        /// </param>
        /// <param name="log">
        /// The logger.
        /// </param>
        /// <returns>
        /// The <see cref="Task" />.
        /// </returns>
        [FunctionName(nameof(TriggerExecutionOrchestrator))]
        public async Task RunOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext orchestrationContext,
            ExecutionContext executionContext,
            ILogger log)
        {
            WorkflowMessageEnvelope envelope =
                orchestrationContext.GetInputWithCustomSerializationSettings<WorkflowMessageEnvelope>(
                    this.serializerSettingsProvider.Instance);

            try
            {
                await orchestrationContext.CallActivityAsync(
                    nameof(StartLongRunningOperationActivity),
                    (envelope.OperationId, envelope.TenantId));

                if (envelope.IsStartWorkflowRequest)
                {
                    if (!orchestrationContext.IsReplaying)
                    {
                        log.LogDebug(
                                $"Received new start workflow request with Id {envelope.StartWorkflowInstanceRequest.RequestId} for workflow {envelope.StartWorkflowInstanceRequest.WorkflowId}");
                    }

                    await orchestrationContext.CallActivityWithCustomSerializationSettingsAsync(
                        nameof(CreateWorkflowActivity),
                        envelope,
                        this.serializerSettingsProvider.Instance);
                }
                else
                {
                    if (!orchestrationContext.IsReplaying)
                    {
                        log.LogDebug($"Received new workflow trigger with Id {envelope.Trigger.Id}");
                    }

                    int count = await orchestrationContext.CallActivityWithCustomSerializationSettingsAsync<WorkflowMessageEnvelope, int>(
                                    nameof(GetWorkflowInstanceCountActivity),
                                    envelope,
                                    this.serializerSettingsProvider.Instance);

                    int pages = (int)Math.Ceiling((decimal)count / 500);

                    if (!orchestrationContext.IsReplaying)
                    {
                        log.LogDebug($"Found {count} instances that match. Split into {pages} pages for fan-out.");
                    }

                    var tasks = new Task[pages];

                    for (int i = 0; i < pages; i++)
                    {
                        envelope.SetWorkflowInstancesPageNumber(i);
                        tasks[i] = orchestrationContext.CallSubOrchestratorWithCustomSerializationSettingsAsync(
                            nameof(TriggerInstancesExecutionOrchestrator),
                            envelope,
                            this.serializerSettingsProvider.Instance);
                    }

                    await Task.WhenAll(tasks);

                    if (!orchestrationContext.IsReplaying)
                    {
                        log.LogDebug("All sub-orchestrations complete");
                    }
                }

                await orchestrationContext.CallActivityAsync(
                    nameof(CompleteLongRunningOperationActivity),
                    (envelope.OperationId, envelope.TenantId));
            }
            catch (FunctionFailedException x)
            {
                if (!orchestrationContext.IsReplaying)
                {
                    log.LogError($"Error during orchestration: {x}");
                }

                await orchestrationContext.CallActivityAsync(
                    nameof(FailLongRunningOperationActivity),
                    (envelope.OperationId, envelope.TenantId));

                throw;
            }
        }
    }
}