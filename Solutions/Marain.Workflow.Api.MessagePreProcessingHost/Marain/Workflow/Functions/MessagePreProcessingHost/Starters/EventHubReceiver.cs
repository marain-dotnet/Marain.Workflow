// <copyright file="EventHubReceiver.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflow.Functions.MessagePreProcessingHost.Starters
{
    using System.Threading.Tasks;

    using Marain.Composition;
    using Marain.Workflow.Functions.MessagePreProcessingHost.Orchestrators;
    using Marain.Workflow.Functions.MessagePreProcessingHost.Shared;

    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.DataContracts;
    using Microsoft.Azure.EventHubs;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    /// <summary>
    ///     Function to receive incoming messages from the event hub and start an orchestration
    ///     for each.
    /// </summary>
    public static class EventHubReceiver
    {
        /// <summary>
        ///     The entry point for the function.
        /// </summary>
        /// <param name="myEventHubMessage">
        ///     The serialized event hub message.
        /// </param>
        /// <param name="orchestrationClient">
        ///     The orchestration client with which a durable function can be started.
        /// </param>
        /// <param name="context">
        ///     The request context.
        /// </param>
        /// <param name="log">
        ///     The logger.
        /// </param>
        /// <returns>
        ///     A <see cref="Task" /> which completes when the message has been processed.
        /// </returns>
        [FunctionName(nameof(EventHubReceiver))]
        public static async Task Run(
            [EventHubTrigger("endworkflow", Connection = "EventHubConnectionString")]
            EventData myEventHubMessage,
            [OrchestrationClient] DurableOrchestrationClient orchestrationClient,
            ExecutionContext context,
            ILogger log)
        {
            log.LogInformation($"EventHubReceiver starter function processed message: {myEventHubMessage}");

            Initialization.Initialize(context);

            log.LogDebug($"Extracting data from event hub message");
            var data = ParameterDataWithTelemetry.FromEventData(myEventHubMessage);

            data.InitializeTelemetryOperationContext(context, log);

            TelemetryClient telemetryClient = ServiceRoot.ServiceProvider.GetService<TelemetryClient>();

            using (telemetryClient.StartOperation<RequestTelemetry>(context.GetFunctionNameForBespokeStartOperation()))
            {
                string instanceId = await orchestrationClient.StartNewAsync(
                                     nameof(TriggerExecutionOrchestrator),
                                     data).ConfigureAwait(false);

                log.LogInformation(
                    $"Started new instance {instanceId} of orchestration function {nameof(TriggerExecutionOrchestrator)}");
            }
        }
    }
}