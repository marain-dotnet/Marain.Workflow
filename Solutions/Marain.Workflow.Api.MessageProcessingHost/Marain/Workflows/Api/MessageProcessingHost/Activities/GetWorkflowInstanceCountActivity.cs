// <copyright file="GetWorkflowInstanceCountActivity.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

#pragma warning disable RCS1090 // Call 'ConfigureAwait(false)'
namespace Marain.Workflows.Api.MessageProcessingHost.Activities
{
    using System.Threading.Tasks;
    using Corvus.Extensions.Json;
    using Corvus.Tenancy;
    using Marain.Workflows;
    using Marain.Workflows.Api.MessageProcessingHost.Shared;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.DurableTask;

    /// <summary>
    /// The create workflow activity.
    /// </summary>
    public class GetWorkflowInstanceCountActivity
    {
        private readonly ITenantedWorkflowInstanceInterestsIndexFactory workflowInstanceInterestsIndexFactory;
        private readonly ITenantProvider tenantProvider;
        private readonly IJsonSerializerSettingsProvider serializerSettingsProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetWorkflowInstanceCountActivity"/> class.
        /// </summary>
        /// <param name="workflowInstanceIndexFactory">The factory class for the workflow engine.</param>
        /// <param name="serializerSettingsProvider">The serialization settings provider.</param>
        /// <param name="tenantProvider">The tenant provider.</param>
        public GetWorkflowInstanceCountActivity(
            ITenantedWorkflowInstanceInterestsIndexFactory workflowInstanceIndexFactory,
            IJsonSerializerSettingsProvider serializerSettingsProvider,
            ITenantProvider tenantProvider)
        {
            this.workflowInstanceInterestsIndexFactory = workflowInstanceIndexFactory;
            this.tenantProvider = tenantProvider;
            this.serializerSettingsProvider = serializerSettingsProvider;
        }

        /// <summary>
        /// The run action.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="Task" />.
        /// </returns>
        [FunctionName(nameof(GetWorkflowInstanceCountActivity))]
        public async Task<int> RunAction(
            [ActivityTrigger] IDurableActivityContext context)
        {
            WorkflowMessageEnvelope envelope =
                context.GetInputWithCustomSerializationSettings<WorkflowMessageEnvelope>(this.serializerSettingsProvider.Instance);

            ITenant tenant = await this.tenantProvider.GetTenantAsync(envelope.TenantId);

            IWorkflowInstanceInterestsIndexStore instanceStore =
                await this.workflowInstanceInterestsIndexFactory.GetWorkflowInstanceInterestsIndexStoreForTenantAsync(tenant);

            return await instanceStore.GetMatchingWorkflowInstanceCountForSubjectsAsync(envelope.Trigger.GetSubjects());
        }
    }
}