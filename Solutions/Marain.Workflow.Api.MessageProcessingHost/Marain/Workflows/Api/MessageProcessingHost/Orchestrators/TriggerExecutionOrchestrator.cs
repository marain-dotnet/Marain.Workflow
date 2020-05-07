// <copyright file="TriggerExecutionOrchestrator.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

#pragma warning disable RCS1090 // Call 'ConfigureAwait(false)'
namespace Marain.Workflows.Api.MessageProcessingHost.Orchestrators
{
    using System;
    using System.Threading.Tasks;
    using Corvus.Extensions.Json;
    using Corvus.Json;
    using Marain.Workflows.Api.MessageProcessingHost.Activities;
    using Marain.Workflows.Api.MessageProcessingHost.Shared;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.DurableTask;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The trigger execution orchestrator.
    /// </summary>
    public class TriggerExecutionOrchestrator
    {
        private readonly IJsonSerializerSettingsProvider serializerSettingsProvider;
        private readonly IPropertyBagFactory propertyBagFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerExecutionOrchestrator"/> class.
        /// </summary>
        /// <param name="propertyBagFactory">The <see cref="IPropertyBagFactory"/>.</param>
        /// <param name="serializerSettingsProvider">The serialization settings provider.</param>
        public TriggerExecutionOrchestrator(
            IPropertyBagFactory propertyBagFactory,
            IJsonSerializerSettingsProvider serializerSettingsProvider)
        {
            this.serializerSettingsProvider = serializerSettingsProvider;
            this.propertyBagFactory = propertyBagFactory;
        }

        /// <summary>
        /// The run orchestrator.
        /// </summary>
        /// <param name="orchestrationContext">The context.</param>
        /// <param name="log">The logger.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [FunctionName(nameof(TriggerExecutionOrchestrator))]
        public async Task RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext orchestrationContext,
            ILogger log)
        {
            ILogger replaySafeLogger = orchestrationContext.CreateReplaySafeLogger(log);

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
                    replaySafeLogger.LogDebug(
                            $"Received new start workflow request with Id {envelope.StartWorkflowInstanceRequest.RequestId} for workflow {envelope.StartWorkflowInstanceRequest.WorkflowId}");

                    await orchestrationContext.CallActivityWithCustomSerializationSettingsAsync(
                        nameof(CreateWorkflowActivity),
                        envelope,
                        this.serializerSettingsProvider.Instance);
                }
                else
                {
                    replaySafeLogger.LogDebug($"Received new workflow trigger with Id {envelope.Trigger.Id}");

                    int count = await orchestrationContext.CallActivityWithCustomSerializationSettingsAsync<WorkflowMessageEnvelope, int>(
                                    nameof(GetWorkflowInstanceCountActivity),
                                    envelope,
                                    this.serializerSettingsProvider.Instance);

                    int pages = (int)Math.Ceiling((decimal)count / 500);

                    replaySafeLogger.LogDebug($"Found {count} instances that match. Split into {pages} pages for fan-out.");

                    var tasks = new Task[pages];

                    for (int i = 0; i < pages; i++)
                    {
                        envelope.SetWorkflowInstancesPageNumber(i, this.propertyBagFactory);
                        tasks[i] = orchestrationContext.CallSubOrchestratorWithCustomSerializationSettingsAsync(
                            nameof(TriggerInstancesExecutionOrchestrator),
                            envelope,
                            this.serializerSettingsProvider.Instance);
                    }

                    await Task.WhenAll(tasks);

                    replaySafeLogger.LogDebug("All sub-orchestrations complete");
                }

                await orchestrationContext.CallActivityAsync(
                    nameof(CompleteLongRunningOperationActivity),
                    (envelope.OperationId, envelope.TenantId));
            }
            catch (FunctionFailedException x)
            {
                replaySafeLogger.LogError($"Error during orchestration: {x}");

                await orchestrationContext.CallActivityAsync(
                    nameof(FailLongRunningOperationActivity),
                    (envelope.OperationId, envelope.TenantId));

                throw;
            }
        }
    }
}