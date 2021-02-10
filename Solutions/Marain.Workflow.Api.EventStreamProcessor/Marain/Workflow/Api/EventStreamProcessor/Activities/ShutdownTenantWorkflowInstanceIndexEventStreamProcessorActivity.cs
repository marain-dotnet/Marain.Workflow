// <copyright file="ShutdownTenantWorkflowInstanceIndexEventStreamProcessorActivity.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflow.Api.EventStreamProcessor.Activities
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.DurableTask;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Durable activity function which shuts down a running instance of an event processor orchestration.
    /// </summary>
    public class ShutdownTenantWorkflowInstanceIndexEventStreamProcessorActivity
    {
        /// <summary>
        /// Executes the durable activity.
        /// </summary>
        /// <param name="context">The <see cref="IDurableActivityContext" />.</param>
        /// <param name="client">The durable orchestration client.</param>
        /// <param name="targetOrchestrationId">The Id of the orchestration to shut down.</param>
        /// <param name="logger">The current logger.</param>
        /// <returns>A list of tenant Ids.</returns>
        [FunctionName(nameof(ShutdownTenantWorkflowInstanceIndexEventStreamProcessorActivity))]
        public Task<IList<string>> RunAction(
            [ActivityTrigger] IDurableActivityContext context,
            [DurableClient] IDurableOrchestrationClient client,
            string targetOrchestrationId,
            ILogger logger)
        {
            return Task.FromResult<IList<string>>(new List<string>());
        }
    }
}