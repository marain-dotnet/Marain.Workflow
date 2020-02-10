// <copyright file="TriggerInstancesExecutionOrchestrator.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

#pragma warning disable RCS1090 // Call 'ConfigureAwait(false)'
namespace Marain.Workflows.Api.MessagePreProcessingHost.Orchestrators
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Corvus.Extensions.Json;
    using Marain.Workflows.Api.MessagePreProcessingHost.Activities;
    using Marain.Workflows.Api.MessagePreProcessingHost.Shared;

    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.DurableTask;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The trigger instances execution orchestrator.
    /// </summary>
    public class TriggerInstancesExecutionOrchestrator
    {
        private readonly IJsonSerializerSettingsProvider serializerSettingsProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerExecutionOrchestrator"/> class.
        /// </summary>
        /// <param name="serializerSettingsProvider">The serialization settings provider.</param>
        public TriggerInstancesExecutionOrchestrator(IJsonSerializerSettingsProvider serializerSettingsProvider)
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
        [FunctionName(nameof(TriggerInstancesExecutionOrchestrator))]
        public async Task RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext orchestrationContext,
            ExecutionContext executionContext,
            ILogger log)
        {
            ILogger replaySafeLogger = orchestrationContext.CreateReplaySafeLogger(log);

            replaySafeLogger.LogDebug("Starting new TriggerInstancesExecutionOrchestrator instance");

            WorkflowMessageEnvelope envelope =
                orchestrationContext.GetInputWithCustomSerializationSettings<WorkflowMessageEnvelope>(
                    this.serializerSettingsProvider.Instance);

            int pageNumber = envelope.GetWorkflowInstancesPageNumber();

            replaySafeLogger.LogDebug($"Processing trigger {envelope.Trigger.Id} against instances page {pageNumber}");

            string[] instanceIds =
                await orchestrationContext.CallActivityWithCustomSerializationSettingsAsync<WorkflowMessageEnvelope, string[]>(
                    nameof(GetWorkflowInstanceIdsActivity),
                    envelope,
                    this.serializerSettingsProvider.Instance);

            replaySafeLogger.LogDebug($"Retrieved {instanceIds.Length} instance Ids");

            var tasks = new List<Task>(instanceIds.Length);

            foreach (string current in instanceIds)
            {
                envelope.SetWorkflowInstanceId(current);
                tasks.Add(orchestrationContext.CallActivityWithCustomSerializationSettingsAsync(
                    nameof(ProcessTriggerActivity),
                    envelope,
                    this.serializerSettingsProvider.Instance));
            }

            await Task.WhenAll(tasks);

            replaySafeLogger.LogDebug($"Processing trigger {envelope.Trigger.Id} against instances page {pageNumber}");
        }
    }
}