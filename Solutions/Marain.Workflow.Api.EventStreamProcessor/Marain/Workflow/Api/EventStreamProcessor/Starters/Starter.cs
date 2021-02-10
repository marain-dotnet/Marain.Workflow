// <copyright file="Starter.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflow.Api.EventStreamProcessor.Marain.Workflow.Api.EventStreamProcessor.Starters
{
    using System.Threading.Tasks;
    using global::Marain.Workflow.Api.EventStreamProcessor.Orchestrations;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.DurableTask;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Contains the starter function for the processor, which ensures that the required orchestration is running.
    /// </summary>
    public class Starter
    {
        /// <summary>
        /// Executes the function.
        /// </summary>
        /// <param name="myTimer">The timer that causes this function to run.</param>
        /// <param name="client">The durable orchestration client, required to start new orchestrations.</param>
        /// <param name="log">The current logger.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [FunctionName("MarainWorkflow-EventStreamProcessor-Timer")]
        public async Task Run(
            [TimerTrigger("0 */10 * * * *", RunOnStartup = true)] TimerInfo myTimer,
            [DurableClient] IDurableOrchestrationClient client,
            ILogger log)
        {
            log.LogDebug("Attempting to start orchestration '{orchestrationName}'.", nameof(EventStreamProcessorControlOrchestration));

            await client.StartNewAsync(
                nameof(EventStreamProcessorControlOrchestration),
                nameof(EventStreamProcessorControlOrchestration)).ConfigureAwait(false);

            ////else
            ////{
            ////    log.LogDebug(
            ////        "Orchestration '{orchestrationName}' has runtime status '{runtimeStatus}'.",
            ////        nameof(EventStreamProcessorControlOrchestration),
            ////        existingInstance.RuntimeStatus);
            ////}

            ////log.LogDebug(
            ////    "Checking for orchestration '{orchestrationName}' complete. Next check at '{nextCheck}'.",
            ////    nameof(EventStreamProcessorControlOrchestration),
            ////    myTimer.ScheduleStatus.Next);
        }
    }
}
