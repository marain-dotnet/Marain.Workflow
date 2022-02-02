// <copyright file="TriggerInstancesExecutionOrchestrator.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.MessageProcessingHost.Orchestrators
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Corvus.Extensions.Json;
    using Corvus.Json;
    using Marain.Workflows.Api.MessageProcessingHost.Activities;
    using Marain.Workflows.Api.MessageProcessingHost.Shared;

    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.DurableTask;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The trigger instances execution orchestrator.
    /// </summary>
    public class TriggerInstancesExecutionOrchestrator
    {
        private readonly IJsonSerializerSettingsProvider serializerSettingsProvider;
        private readonly IPropertyBagFactory propertyBagFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerExecutionOrchestrator"/> class.
        /// </summary>
        /// <param name="propertyBagFactory">The property bag factory.</param>
        /// <param name="serializerSettingsProvider">The serialization settings provider.</param>
        public TriggerInstancesExecutionOrchestrator(
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
        [FunctionName(nameof(TriggerInstancesExecutionOrchestrator))]
        public async Task RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext orchestrationContext,
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
                envelope.SetWorkflowInstanceId(this.propertyBagFactory, current);
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