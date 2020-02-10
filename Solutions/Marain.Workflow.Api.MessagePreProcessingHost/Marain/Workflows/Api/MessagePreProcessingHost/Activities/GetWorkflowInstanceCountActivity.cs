// <copyright file="GetWorkflowInstanceCountActivity.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

#pragma warning disable RCS1090 // Call 'ConfigureAwait(false)'
namespace Marain.Workflows.Api.MessagePreProcessingHost.Activities
{
    using System.Threading.Tasks;
    using Corvus.Extensions.Json;
    using Corvus.Tenancy;
    using Marain.Workflows;
    using Marain.Workflows.Api.MessagePreProcessingHost.Shared;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The create workflow activity.
    /// </summary>
    public class GetWorkflowInstanceCountActivity
    {
        private readonly IWorkflowEngineFactory workflowEngineFactory;
        private readonly ITenantProvider tenantProvider;
        private readonly IJsonSerializerSettingsProvider serializerSettingsProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetWorkflowInstanceCountActivity"/> class.
        /// </summary>
        /// <param name="workflowEngineFactory">The factory class for the workflow engine.</param>
        /// <param name="serializerSettingsProvider">The serialization settings provider.</param>
        /// <param name="tenantProvider">The tenant provider.</param>
        public GetWorkflowInstanceCountActivity(
            IWorkflowEngineFactory workflowEngineFactory,
            IJsonSerializerSettingsProvider serializerSettingsProvider,
            ITenantProvider tenantProvider)
        {
            this.workflowEngineFactory = workflowEngineFactory;
            this.tenantProvider = tenantProvider;
            this.serializerSettingsProvider = serializerSettingsProvider;
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
        [FunctionName(nameof(GetWorkflowInstanceCountActivity))]
        public async Task<int> RunAction(
            [ActivityTrigger] DurableActivityContext context,
            ExecutionContext executionContext,
            ILogger logger)
        {
            WorkflowMessageEnvelope envelope =
                context.GetInputWithCustomSerializationSettings<WorkflowMessageEnvelope>(this.serializerSettingsProvider.Instance);

            ITenant tenant = await this.tenantProvider.GetTenantAsync(envelope.TenantId);

            IWorkflowEngine workflowEngine = await this.workflowEngineFactory.GetWorkflowEngineAsync(tenant);

            return await workflowEngine.GetMatchingWorkflowInstanceCountForSubjectsAsync(envelope.Trigger.GetSubjects());
        }
    }
}