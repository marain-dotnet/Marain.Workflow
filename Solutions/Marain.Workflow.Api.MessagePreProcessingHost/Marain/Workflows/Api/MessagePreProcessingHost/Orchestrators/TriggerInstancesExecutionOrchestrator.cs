// <copyright file="TriggerInstancesExecutionOrchestrator.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

#pragma warning disable RCS1090 // Call 'ConfigureAwait(false)'
namespace Marain.Workflows.Api.MessagePreProcessingHost.Orchestrators
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Marain.Workflows.Api.MessagePreProcessingHost.Activities;
    using Marain.Workflows.Api.MessagePreProcessingHost.Shared;

    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.DurableTask;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The trigger instances execution orchestrator.
    /// </summary>
    public static class TriggerInstancesExecutionOrchestrator
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
        [FunctionName(nameof(TriggerInstancesExecutionOrchestrator))]
        public static async Task RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext orchestrationContext,
            ExecutionContext executionContext,
            ILogger log)
        {
            ILogger replaySafeLogger = orchestrationContext.CreateReplaySafeLogger(log);

            replaySafeLogger.LogDebug("Starting new TriggerInstancesExecutionOrchestrator instance");

            WorkflowMessageEnvelope envelope = orchestrationContext.GetInput<WorkflowMessageEnvelope>();

            int pageNumber = envelope.GetWorkflowInstancesPageNumber();

            replaySafeLogger.LogDebug($"Processing trigger {envelope.Trigger.Id} against instances page {pageNumber}");

            string[] instanceIds = await orchestrationContext.CallActivityAsync<string[]>(
                                  nameof(GetWorkflowInstanceIdsActivity),
                                  envelope);

            replaySafeLogger.LogDebug($"Retrieved {instanceIds.Length} instance Ids");

            var tasks = new List<Task>(instanceIds.Length);

            foreach (string current in instanceIds)
            {
                envelope.SetWorkflowInstanceId(current);
                tasks.Add(
                    orchestrationContext.CallActivityAsync(
                        nameof(ProcessTriggerActivity),
                        envelope));
            }

            await Task.WhenAll(tasks);

            replaySafeLogger.LogDebug($"Processing trigger {envelope.Trigger.Id} against instances page {pageNumber}");
        }
    }
}