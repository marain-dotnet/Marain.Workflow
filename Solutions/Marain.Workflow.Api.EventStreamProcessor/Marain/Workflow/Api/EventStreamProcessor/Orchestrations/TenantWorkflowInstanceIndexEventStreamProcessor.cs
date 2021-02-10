// <copyright file="TenantWorkflowInstanceIndexEventStreamProcessor.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

#pragma warning disable RCS1090 // Add call to 'ConfigureAwait' (or vice versa).
namespace Marain.Workflow.Api.EventStreamProcessor.Orchestrations
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Corvus.Tenancy;
    using global::Marain.Workflow.Api.EventStreamProcessor.Activities;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.DurableTask;

    /// <summary>
    /// Durable orchestration that handles processing of the event stream for a specific tenant.
    /// </summary>
    public class TenantWorkflowInstanceIndexEventStreamProcessor
    {
        /// <summary>
        /// The prefix that should be used for Ids of instances of this orchestration.
        /// </summary>
        /// <remarks>
        /// Do not use directly. Call <see cref="GetOrchestrationIdForTenantId"/>.</remarks>
        public const string OrchestrationIdPrefix = nameof(TenantWorkflowInstanceIndexEventStreamProcessor);

        /// <summary>
        /// Builds the Id that should be used for the instance of this orchestration for the given tenant Id.
        /// </summary>
        /// <param name="tenantId">The Id of the tenant that this orchestration should process the event stream for.</param>
        /// <returns>The Id of the orchestration.</returns>
        public static string GetOrchestrationIdForTenantId(string tenantId)
        {
            return string.Concat(OrchestrationIdPrefix, tenantId);
        }

        /// <summary>
        /// Extracts the tenant Id that this orchestration is for from the orchestration Id. Relies on the orchestration
        /// Id having been created with <see cref="GetOrchestrationIdForTenantId(string)"/>.
        /// </summary>
        /// <param name="orchestrationId">The orchestration Id.</param>
        /// <returns>The Id of the tenant.</returns>
        public static string GetTenantIdFromOrchestrationId(string orchestrationId)
        {
            return orchestrationId[OrchestrationIdPrefix.Length..];
        }

        /// <summary>
        /// Runs the orchestrator function.
        /// </summary>
        /// <param name="context">The current <see cref="IDurableOrchestrationContext"/>.</param>
        /// <param name="tenantId">The Id of the tenant that this orchestration is for.</param>
        /// <returns>A task which completes when the orchestration has completed.</returns>
        [FunctionName(nameof(TenantWorkflowInstanceIndexEventStreamProcessor))]
        public async Task RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            string tenantId)
        {
            ITenant tenant = await context.CallActivityAsync<ITenant>(nameof(GetTenantByIdActivity), tenantId);

            int processedEventsCount = await context.CallActivityAsync<int>(nameof(ProcessEventBatchActivity), tenant);

            // If no events were processed, back off. Otherwise go around again immediately.
            if (processedEventsCount == 0)
            {
                await context.CreateTimer(context.CurrentUtcDateTime.AddSeconds(5), CancellationToken.None);
            }

            context.ContinueAsNew(tenantId);
        }
    }
}
