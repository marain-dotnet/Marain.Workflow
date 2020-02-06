// <copyright file="GetWorkflowInstanceIdsActivity.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.MessagePreProcessingHost.Activities
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Corvus.Tenancy;
    using Marain.Workflows.Api.MessagePreProcessingHost.Shared;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.DurableTask;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The create workflow activity.
    /// </summary>
    public class GetWorkflowInstanceIdsActivity
    {
        private readonly IWorkflowEngineFactory workflowEngineFactory;
        private readonly ITenantProvider tenantProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetWorkflowInstanceCountActivity"/> class.
        /// </summary>
        /// <param name="workflowEngineFactory">The factory class for the workflow engine.</param>
        /// <param name="tenantProvider">The tenant provider.</param>
        public GetWorkflowInstanceIdsActivity(
            IWorkflowEngineFactory workflowEngineFactory,
            ITenantProvider tenantProvider)
        {
            this.workflowEngineFactory = workflowEngineFactory;
            this.tenantProvider = tenantProvider;
        }

        /// <summary>
        /// The run action.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <param name="executionContext">
        /// The execution Context.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <returns>
        /// The <see cref="Task" />.
        /// </returns>
        [FunctionName(nameof(GetWorkflowInstanceIdsActivity))]
        public async Task<string[]> RunAction(
            [ActivityTrigger] IDurableActivityContext context,
            ExecutionContext executionContext,
            ILogger logger)
        {
            WorkflowMessageEnvelope envelope = context.GetInput<WorkflowMessageEnvelope>();
            ITenant tenant = await this.tenantProvider.GetTenantAsync(envelope.TenantId).ConfigureAwait(false);

            IWorkflowEngine workflowEngine = await this.workflowEngineFactory.GetWorkflowEngineAsync(tenant).ConfigureAwait(false);
            IEnumerable<string> instanceIds = await workflowEngine.GetMatchingWorkflowInstanceIdsForSubjectsAsync(
                                    envelope.Trigger.GetSubjects(),
                                    500,
                                    envelope.GetWorkflowInstancesPageNumber()).ConfigureAwait(false);

            return instanceIds.ToArray();
        }
    }
}