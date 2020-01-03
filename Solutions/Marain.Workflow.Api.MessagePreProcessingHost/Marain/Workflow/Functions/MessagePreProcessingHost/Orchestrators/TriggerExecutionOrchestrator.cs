// <copyright file="TriggerExecutionOrchestrator.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflow.Functions.MessagePreProcessingHost.Orchestrators
{
    using System;
    using System.Threading.Tasks;
    using Marain.Workflow.Functions.MessagePreProcessingHost.Activities;
    using Marain.Workflow.Functions.MessagePreProcessingHost.Shared;

    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;

    /// <summary>
    ///     The trigger execution orchestrator.
    /// </summary>
    public static class TriggerExecutionOrchestrator
    {
        /// <summary>
        ///     The run orchestrator.
        /// </summary>
        /// <param name="orchestrationContext">
        ///     The context.
        /// </param>
        /// <param name="executionContext">
        ///     The execution context.
        /// </param>
        /// <param name="log">
        ///     The logger.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        [FunctionName(nameof(TriggerExecutionOrchestrator))]
        public static async Task RunOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext orchestrationContext,
            ExecutionContext executionContext,
            ILogger log)
        {
            Initialization.Initialize(executionContext);

            ParameterDataWithTelemetry parameterData = orchestrationContext.GetInput<ParameterDataWithTelemetry>();
            parameterData.InitializeTelemetryOperationContext(executionContext, log);

            Workflows.WorkflowMessageEnvelope envelope = parameterData.GetWorkflowMessageEnvelope();

            try
            {
#pragma warning disable RCS1090 // Call 'ConfigureAwait(false)'.
                await orchestrationContext.CallActivityAsync(
                    nameof(StartLongRunningOperationActivity),
                    envelope.OperationId);
#pragma warning restore RCS1090 // Call 'ConfigureAwait(false)'.

                if (envelope.IsStartWorkflowRequest)
                {
                    log.LogDebugIf(
                        !orchestrationContext.IsReplaying,
                        () =>
                            $"Received new start workflow request with Id {envelope.StartWorkflowInstanceRequest.RequestId} for workflow {envelope.StartWorkflowInstanceRequest.WorkflowId}");

#pragma warning disable RCS1090 // Call 'ConfigureAwait(false)'.
                    await orchestrationContext.CallActivityAsync(
                        nameof(CreateWorkflowActivity),
                        parameterData);
#pragma warning restore RCS1090 // Call 'ConfigureAwait(false)'.
                }
                else
                {
                    log.LogDebugIf(
                        !orchestrationContext.IsReplaying,
                        () => $"Received new workflow trigger with Id {envelope.Trigger.Id}");

#pragma warning disable RCS1090 // Call 'ConfigureAwait(false)'.
                    int count = await orchestrationContext.CallActivityAsync<int>(
                                    nameof(GetWorkflowInstanceCountActivity),
                                    parameterData);
#pragma warning restore RCS1090 // Call 'ConfigureAwait(false)'.

                    int pages = (int)Math.Ceiling((decimal)count / 500);

                    log.LogDebugIf(
                        !orchestrationContext.IsReplaying,
                        () => $"Found {count} instances that match. Split into {pages} pages for fan-out.");

                    var tasks = new Task[pages];

                    for (int i = 0; i < pages; i++)
                    {
                        parameterData.SetWorkflowInstancesPageNumber(i);
                        tasks[i] = orchestrationContext.CallSubOrchestratorAsync(
                            nameof(TriggerInstancesExecutionOrchestrator),
                            parameterData);
                    }

#pragma warning disable RCS1090 // Call 'ConfigureAwait(false)'.
                    await Task.WhenAll(tasks);
#pragma warning restore RCS1090 // Call 'ConfigureAwait(false)'.

                    log.LogDebugIf(!orchestrationContext.IsReplaying, () => $"All sub-orchestrations complete");
                }

#pragma warning disable RCS1090 // Call 'ConfigureAwait(false)'.
                await orchestrationContext.CallActivityAsync(
                    nameof(CompleteLongRunningOperationActivity),
                    envelope.OperationId);
#pragma warning restore RCS1090 // Call 'ConfigureAwait(false)'.
            }
            catch (FunctionFailedException x)
            {
                log.LogErrorIf(!orchestrationContext.IsReplaying, () => $"Error during orchestration: {x}");
#pragma warning disable RCS1090 // Call 'ConfigureAwait(false)'.
                await orchestrationContext.CallActivityAsync(
                    nameof(FailLongRunningOperationActivity),
                    envelope.OperationId);
#pragma warning restore RCS1090 // Call 'ConfigureAwait(false)'.

                throw;
            }
        }
    }
}