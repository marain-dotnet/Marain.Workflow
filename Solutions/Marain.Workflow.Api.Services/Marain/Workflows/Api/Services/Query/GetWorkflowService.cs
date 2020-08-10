// <copyright file="GetWorkflowService.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.Services.Query
{
    using System.Threading.Tasks;
    using Corvus.Tenancy;
    using Marain.Services.Tenancy;
    using Menes;

    /// <summary>
    /// Implements the get workflows endpoint for the query API.
    /// </summary>
    public class GetWorkflowService : IOpenApiService
    {
        /// <summary>
        /// The operation Id for the endpoint.
        /// </summary>
        public const string GetWorkflowOperationId = "getWorkflow";

        private readonly IMarainServicesTenancy marainServicesTenancy;
        private readonly ITenantedWorkflowStoreFactory workflowStoreFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetWorkflowService"/> class.
        /// </summary>
        /// <param name="workflowStoreFactory">The workflow store factory.</param>
        /// <param name="marainServicesTenancy">The tenancy services.</param>
        public GetWorkflowService(
            ITenantedWorkflowStoreFactory workflowStoreFactory,
            IMarainServicesTenancy marainServicesTenancy)
        {
            this.marainServicesTenancy = marainServicesTenancy;
            this.workflowStoreFactory = workflowStoreFactory;
        }

        /// <summary>
        /// Retrieves a specific workflow.
        /// </summary>
        /// <param name="context">The current OpenApi context.</param>
        /// <param name="workflowId">The Id of the workflow to retrieve.</param>
        /// <returns>The workflow, as an OpenApiResult.</returns>
        [OperationId(GetWorkflowOperationId)]
        public async Task<OpenApiResult> GetWorkflowAsync(
            IOpenApiContext context,
            string workflowId)
        {
            ITenant tenant = await this.marainServicesTenancy.GetRequestingTenantAsync(context.CurrentTenantId).ConfigureAwait(false);
            IWorkflowStore workflowStore = await this.workflowStoreFactory.GetWorkflowStoreForTenantAsync(tenant).ConfigureAwait(false);
            Workflow workflow = await workflowStore.GetWorkflowAsync(workflowId).ConfigureAwait(false);
            OpenApiResult result = this.OkResult(workflow, "application/json");

            if (!string.IsNullOrEmpty(workflow.ETag))
            {
                result.Results.Add("ETag", workflow.ETag);
            }

            return result;
        }
    }
}
