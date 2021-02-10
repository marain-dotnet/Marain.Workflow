// <copyright file="EventStreamProcessorControlOrchestration.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

#pragma warning disable RCS1090 // Add call to 'ConfigureAwait' (or vice versa).
namespace Marain.Workflow.Api.EventStreamProcessor.Orchestrations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using global::Marain.Workflow.Api.EventStreamProcessor.Activities;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.DurableTask;

    /// <summary>
    /// Contains the durable orchestration that controls instances of the event stream processor orchestration.
    /// </summary>
    public class EventStreamProcessorControlOrchestration
    {
        /// <summary>
        /// Runs the orchestrator function.
        /// </summary>
        /// <param name="context">The current <see cref="IDurableOrchestrationContext"/>.</param>
        /// <returns>A task which completes when the orchestration has completed.</returns>
        [FunctionName(nameof(EventStreamProcessorControlOrchestration))]
        public async Task RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            // Get the tenants who are enrolled for this service.
            IList<string> tenants = await context.CallActivityAsync<IList<string>>(nameof(GetEnrolledWorkflowTenantsActivity), null);

            // Get list of currently running instances of the TenantWorkflowInstanceIndexEventStreamProcessor
            IList<string> runningProcessorIds = await context.CallActivityAsync<IList<string>>(nameof(GetRunningProcessorIdsActivity), TenantWorkflowInstanceIndexEventStreamProcessor.OrchestrationIdPrefix);

            // Start new instances where needed
            IEnumerable<string> tenantsWithoutRunningProcessors = tenants.Where(t => !runningProcessorIds.Contains(TenantWorkflowInstanceIndexEventStreamProcessor.GetOrchestrationIdForTenantId(t)));
            tenantsWithoutRunningProcessors.Select(t => context.StartNewOrchestration(nameof(TenantWorkflowInstanceIndexEventStreamProcessor), t, TenantWorkflowInstanceIndexEventStreamProcessor.GetOrchestrationIdForTenantId(t)));

            // Shut down instances where needed
            IEnumerable<string> runningProcessorsWithoutTenants = runningProcessorIds.Where(p => !tenants.Contains(TenantWorkflowInstanceIndexEventStreamProcessor.GetTenantIdFromOrchestrationId(p)));

            IEnumerable<Task> shutdownTasks = runningProcessorsWithoutTenants.Select(p => context.CallActivityAsync(nameof(ShutdownTenantWorkflowInstanceIndexEventStreamProcessorActivity), p));

            await Task.WhenAll(shutdownTasks);

            // Now we want to run this again in 10 minutes.
            await context.CreateTimer(context.CurrentUtcDateTime.AddMinutes(10), CancellationToken.None);

            context.ContinueAsNew(null);
        }
    }
}