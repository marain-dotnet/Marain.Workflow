// <copyright file="GetWorkflowStatesService.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.Services.Query
{
    using System.Threading.Tasks;
    using Corvus.Tenancy;
    using Marain.Services.Tenancy;
    using Menes;

    /// <summary>
    /// Implements the get workflow states endpoint for the query API.
    /// </summary>
    public class GetWorkflowStatesService : IOpenApiService
    {
        /// <summary>
        /// The operation Id for the endpoint.
        /// </summary>
        public const string GetWorkflowStatesOperationId = "getWorkflowStates";

        private readonly IMarainServicesTenancy marainServicesTenancy;
        private readonly ITenantedWorkflowStoreFactory workflowStoreFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetWorkflowService"/> class.
        /// </summary>
        /// <param name="workflowStoreFactory">The workflow store factory.</param>
        /// <param name="marainServicesTenancy">The tenancy services.</param>
        public GetWorkflowStatesService(
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
        /// <param name="maxItems">The maximum number of items to return.</param>
        /// <param name="continuationToken">A continuation token from a previous request.</param>
        /// <returns>The workflow states, as an OpenApiResult.</returns>
        [OperationId(GetWorkflowStatesOperationId)]
        public async Task<OpenApiResult> GetWorkflowStatesAsync(
            IOpenApiContext context,
            string workflowId,
            int? maxItems,
            string continuationToken)
        {
            ITenant tenant = await this.marainServicesTenancy.GetRequestingTenantAsync(context.CurrentTenantId).ConfigureAwait(false);
            IWorkflowStore workflowStore = await this.workflowStoreFactory.GetWorkflowStoreForTenantAsync(tenant).ConfigureAwait(false);
            Workflow workflow = await workflowStore.GetWorkflowAsync(workflowId).ConfigureAwait(false);

            return this.NotImplementedResult();
        }
    }
}
