// <copyright file="TriggerExecutionOrchestrator.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

#pragma warning disable RCS1090 // Call 'ConfigureAwait(false)'
namespace Marain.Workflows.Api.MessagePreProcessingHost.Orchestrators
{
    using System;
    using System.Threading.Tasks;
    using Marain.Workflows.Api.MessagePreProcessingHost.Activities;
    using Marain.Workflows.Api.MessagePreProcessingHost.Shared;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.DurableTask;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The trigger execution orchestrator.
    /// </summary>
    public static class TriggerExecutionOrchestrator
    {
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
        public static async Task RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext orchestrationContext,
            ExecutionContext executionContext,
            ILogger log)
        {
            WorkflowMessageEnvelope envelope = orchestrationContext.GetInput<WorkflowMessageEnvelope>();
            ILogger replaySafeLogger = orchestrationContext.CreateReplaySafeLogger(log);

            try
            {
                await orchestrationContext.CallActivityAsync(
                    nameof(StartLongRunningOperationActivity),
                    (envelope.OperationId, envelope.TenantId));

                if (envelope.IsStartWorkflowRequest)
                {
                    replaySafeLogger.LogDebug(
                            $"Received new start workflow request with Id {envelope.StartWorkflowInstanceRequest.RequestId} for workflow {envelope.StartWorkflowInstanceRequest.WorkflowId}");

                    await orchestrationContext.CallActivityAsync(
                        nameof(CreateWorkflowActivity),
                        envelope);
                }
                else
                {
                    replaySafeLogger.LogDebug($"Received new workflow trigger with Id {envelope.Trigger.Id}");

                    int count = await orchestrationContext.CallActivityAsync<int>(
                                    nameof(GetWorkflowInstanceCountActivity),
                                    envelope);

                    int pages = (int)Math.Ceiling((decimal)count / 500);

                    replaySafeLogger.LogDebug($"Found {count} instances that match. Split into {pages} pages for fan-out.");

                    var tasks = new Task[pages];

                    for (int i = 0; i < pages; i++)
                    {
                        envelope.SetWorkflowInstancesPageNumber(i);
                        tasks[i] = orchestrationContext.CallSubOrchestratorAsync(
                            nameof(TriggerInstancesExecutionOrchestrator),
                            envelope);
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