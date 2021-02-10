// <copyright file="GetTenantByIdActivity.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflow.Api.EventStreamProcessor.Activities
{
    using System;
    using System.Threading.Tasks;
    using Corvus.Tenancy;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.DurableTask;

    /// <summary>
    /// Durable activity function which retrieves a specific tenant.
    /// </summary>
    public class GetTenantByIdActivity
    {
        private readonly ITenantStore tenantProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetTenantByIdActivity"/> class.
        /// </summary>
        /// <param name="tenantProvider">The current tenant provider.</param>
        public GetTenantByIdActivity(
            ITenantStore tenantProvider)
        {
            this.tenantProvider = tenantProvider
                ?? throw new ArgumentNullException(nameof(tenantProvider));
        }

        /// <summary>
        /// Executes the durable activity.
        /// </summary>
        /// <param name="context">The <see cref="IDurableActivityContext" />.</param>
        /// <param name="tenantId">The Id of the tenant to retrieve.</param>
        /// <returns>A list of tenant Ids.</returns>
        [FunctionName(nameof(GetTenantByIdActivity))]
        public Task<ITenant> RunAction(
            [ActivityTrigger] IDurableActivityContext context,
            string tenantId)
        {
            return this.tenantProvider.GetTenantAsync(tenantId);
        }
    }
}
