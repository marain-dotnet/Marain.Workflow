// <copyright file="TriggerInstancesExecutionOrchestrator.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflow.Functions.MessagePreProcessingHost.Orchestrators
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Marain.Workflow.Functions.MessagePreProcessingHost.Activities;
    using Marain.Workflow.Functions.MessagePreProcessingHost.Shared;

    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;

    /// <summary>
    ///     The trigger instances execution orchestrator.
    /// </summary>
    public static class TriggerInstancesExecutionOrchestrator
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
        [FunctionName(nameof(TriggerInstancesExecutionOrchestrator))]
        public static async Task RunOrchestrator(
            [OrchestrationTrigger] DurableOrchestrationContext orchestrationContext,
            ExecutionContext executionContext,
            ILogger log)
        {
            log.LogDebugIf(
                !orchestrationContext.IsReplaying,
                () => "Starting new TriggerInstancesExecutionOrchestrator instance");

            Initialization.Initialize(executionContext);

            ParameterDataWithTelemetry parameterData = orchestrationContext.GetInput<ParameterDataWithTelemetry>();
            parameterData.InitializeTelemetryOperationContext(executionContext, log);

            Workflows.WorkflowMessageEnvelope envelope = parameterData.GetWorkflowMessageEnvelope();
            int pageNumber = parameterData.GetWorkflowInstancesPageNumber();

            log.LogDebugIf(
                !orchestrationContext.IsReplaying,
                () => $"Processing trigger {envelope.Trigger.Id} against instances page {pageNumber}");

#pragma warning disable RCS1090 // Call 'ConfigureAwait(false)'.
            string[] instanceIds = await orchestrationContext.CallActivityAsync<string[]>(
                                  nameof(GetWorkflowInstanceIdsActivity),
                                  parameterData);
#pragma warning restore RCS1090 // Call 'ConfigureAwait(false)'.

            log.LogDebugIf(!orchestrationContext.IsReplaying, () => $"Retrieved {instanceIds.Length} instance Ids");

            var tasks = new List<Task>(instanceIds.Length);

            foreach (string current in instanceIds)
            {
                parameterData.SetWorkflowInstanceId(current);
                tasks.Add(
                    orchestrationContext.CallActivityAsync(
                        nameof(ProcessTriggerActivity),
                        parameterData));
            }

#pragma warning disable RCS1090 // Call 'ConfigureAwait(false)'.
            await Task.WhenAll(tasks);
#pragma warning restore RCS1090 // Call 'ConfigureAwait(false)'.

            log.LogDebugIf(
                !orchestrationContext.IsReplaying,
                () => $"Processing trigger {envelope.Trigger.Id} against instances page {pageNumber}");
        }
    }
}