// <copyright file="GetRunningProcessorIdsActivity.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflow.Api.EventStreamProcessor.Activities
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.DurableTask;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Durable activity function which retrieves a list of tenants that are enrolled to use the workflow engine.
    /// </summary>
    public class GetRunningProcessorIdsActivity
    {
        /// <summary>
        /// Executes the durable activity.
        /// </summary>
        /// <param name="client">The durable orchestration client.</param>
        /// <param name="context">The <see cref="IDurableActivityContext" />.</param>
        /// <param name="targetOrchestrationIdPrefix">The prefix used for Ids of the target orchestration.</param>
        /// <returns>A list of tenant Ids.</returns>
        [FunctionName(nameof(GetRunningProcessorIdsActivity))]
        public async Task<IList<string>> RunAction(
            [DurableClient] IDurableOrchestrationClient client,
            [ActivityTrigger] IDurableActivityContext context,
            string targetOrchestrationIdPrefix)
        {
            var filter = new OrchestrationStatusQueryCondition
            {
                InstanceIdPrefix = targetOrchestrationIdPrefix,
                RuntimeStatus = new[] { OrchestrationRuntimeStatus.ContinuedAsNew, OrchestrationRuntimeStatus.Pending, OrchestrationRuntimeStatus.Running },
            };

            var activeOrchestrationInstanceIds = new List<string>();

            ////logger.LogDebug("Requesting existing running processors with instance Id prefix '{instanceIdPrefix}'}'.", targetOrchestrationIdPrefix);

            do
            {
                OrchestrationStatusQueryResult results = await client.ListInstancesAsync(filter, CancellationToken.None).ConfigureAwait(false);

                activeOrchestrationInstanceIds.AddRange(results.DurableOrchestrationState.Select(x => x.InstanceId));

                filter.ContinuationToken = results.ContinuationToken;
            }
            while (!string.IsNullOrEmpty(filter.ContinuationToken));

            ////logger.LogDebug("Found {existingRunningProcessorCount} existing processors with instance Id prefix '{instanceIdPrefx}'", activeOrchestrationInstanceIds.Count, targetOrchestrationIdPrefix);

            return activeOrchestrationInstanceIds;
        }
    }
}