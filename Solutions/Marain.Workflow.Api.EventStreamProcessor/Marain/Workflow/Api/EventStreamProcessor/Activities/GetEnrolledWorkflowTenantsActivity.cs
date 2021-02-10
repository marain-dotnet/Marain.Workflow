// <copyright file="GetEnrolledWorkflowTenantsActivity.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflow.Api.EventStreamProcessor.Activities
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Corvus.Tenancy;
    using global::Marain.Services;
    using global::Marain.TenantManagement;
    using global::Marain.Workflow.Api.EventStreamProcessor.Configuration;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.DurableTask;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Durable activity function which retrieves a list of tenants that are enrolled to use the workflow engine.
    /// </summary>
    public class GetEnrolledWorkflowTenantsActivity
    {
        private readonly ITenantStore tenantProvider;
        private readonly MarainServiceConfiguration serviceConfiguration;
        private readonly EventStreamProcessorConfiguration eventStreamProcessorConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetEnrolledWorkflowTenantsActivity"/> class.
        /// </summary>
        /// <param name="tenantProvider">The current tenant provider.</param>
        /// <param name="eventStreamProcessorConfiguration">Configuration for the event stream processor service.</param>
        /// <param name="serviceConfiguration">The configuration for the workflow service.</param>
        public GetEnrolledWorkflowTenantsActivity(
            ITenantStore tenantProvider,
            EventStreamProcessorConfiguration eventStreamProcessorConfiguration,
            MarainServiceConfiguration serviceConfiguration)
        {
            this.tenantProvider = tenantProvider
                ?? throw new ArgumentNullException(nameof(tenantProvider));

            this.serviceConfiguration = serviceConfiguration
                ?? throw new ArgumentNullException(nameof(serviceConfiguration));

            this.eventStreamProcessorConfiguration = eventStreamProcessorConfiguration
                ?? throw new ArgumentNullException(nameof(eventStreamProcessorConfiguration));
        }

        /// <summary>
        /// Executes the durable activity.
        /// </summary>
        /// <param name="context">The <see cref="IDurableActivityContext" />.</param>
        /// <returns>A list of tenant Ids.</returns>
        [FunctionName(nameof(GetEnrolledWorkflowTenantsActivity))]
        public async Task<IList<string>> RunAction(
            [ActivityTrigger] IDurableActivityContext context)
        {
            // TODO: Error handling
            // TODO: Logging
            var enrolledTenantRetrievalTasks = new List<Task<IList<string>>>
            {
                this.GetEnrolledClientTenantIds(),
            };

            // TODO: We could do this by scanning the manifests of service tenants rather than requiring configuration...
            // this would be cacheable so would only be expensive first time round.
            enrolledTenantRetrievalTasks.AddRange(this.eventStreamProcessorConfiguration.WorkflowDependentServiceTenantIds.Select(t => this.GetAllChildTenantIds(t, false)));

            IList<string>[] allEnrolledTenantIdLists = await Task.WhenAll(enrolledTenantRetrievalTasks).ConfigureAwait(false);
            return allEnrolledTenantIdLists.SelectMany(x => x).ToList();
        }

        private async Task<IList<string>> GetEnrolledClientTenantIds()
        {
            IList<string> clientTenantIds = await this.GetAllChildTenantIds(WellKnownTenantIds.ClientTenantParentId, this.eventStreamProcessorConfiguration.RecursiveTenantSearch).ConfigureAwait(false);
            return await this.GetTenantIdsEnrolledForWorkflow(clientTenantIds).ConfigureAwait(false);
        }

        private async Task<IList<string>> GetAllChildTenantIds(
            string parentTenantId,
            bool recurse)
        {
            string? continuationToken = null;
            var tenantIds = new List<string>();

            do
            {
                TenantCollectionResult results = await this.tenantProvider.GetChildrenAsync(
                    parentTenantId,
                    100,
                    continuationToken).ConfigureAwait(false);

                tenantIds.AddRange(results.Tenants);

                continuationToken = results.ContinuationToken;
            }
            while (!string.IsNullOrEmpty(continuationToken));

            if (recurse)
            {
                // Now get all the child tenants...
                IList<string>[] children = await Task.WhenAll(tenantIds.Select(t => this.GetAllChildTenantIds(t, true))).ConfigureAwait(false);
                children.ForEach(c => tenantIds.AddRange(c));
            }

            return tenantIds;
        }

        private async Task<IList<string>> GetTenantIdsEnrolledForWorkflow(IList<string> tenantIds)
        {
            if (this.eventStreamProcessorConfiguration.CheckTenantEnrollmentStatus)
            {
                bool[] enrollmentStatuses = await Task.WhenAll(tenantIds.Select(t => this.IsTenantIdEnrolledForWorkflow(t))).ConfigureAwait(false);

                return tenantIds.Where((t, i) => enrollmentStatuses[i]).ToList();
            }

            return tenantIds;
        }

        private async Task<bool> IsTenantIdEnrolledForWorkflow(string tenantId)
        {
            ITenant tenant = await this.tenantProvider.GetTenantAsync(tenantId).ConfigureAwait(false);

            return tenant.IsEnrolledForService(this.serviceConfiguration.ServiceTenantId);
        }
    }
}
