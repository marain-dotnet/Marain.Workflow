// <copyright file="EngineService.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.Services
{
    using System.Threading.Tasks;

    using Corvus.Tenancy;

    using Marain.Services.Tenancy;

    using Menes;

    /// <summary>
    /// Handles incoming triggers posted to the trigger service.
    /// </summary>
    [EmbeddedOpenApiDefinition("Marain.Workflows.Api.Services.EngineService.yaml")]
    public class EngineService : IOpenApiService
    {
        private const string StartWorkflowInstanceOperationId = "startWorkflowInstance";
        private const string SendTriggerOperationId = "sendTrigger";
        private const string CreateWorkflowOperationId = "createWorkflow";
        private const string UpdateWorkflowOperationId = "updateWorkflow";
        private const string GetWorkflowOperationId = "getWorkflow";

        private readonly IMarainServicesTenancy marainServicesTenancy;
        private readonly ITenantedWorkflowEngineFactory workflowEngineFactory;
        private readonly ITenantedWorkflowStoreFactory workflowStoreFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="EngineService"/> class.
        /// </summary>
        /// <param name="workflowEngineFactory">The workflow engine factory.</param>
        /// <param name="workflowStoreFactory">The workflow store factory.</param>
        /// <param name="marainServicesTenancy">The tenancy services.</param>
        public EngineService(
            ITenantedWorkflowEngineFactory workflowEngineFactory,
            ITenantedWorkflowStoreFactory workflowStoreFactory,
            IMarainServicesTenancy marainServicesTenancy)
        {
            this.workflowEngineFactory = workflowEngineFactory;
            this.marainServicesTenancy = marainServicesTenancy;
            this.workflowStoreFactory = workflowStoreFactory;
        }

        /// <summary>
        /// Handles incoming triggers for an instance.
        /// </summary>
        /// <param name="tenantId">The tenant Id for the current request.</param>
        /// <param name="workflowInstanceId">The Id of the workflow instance to which this trigger will be applied.</param>
        /// <param name="body">The trigger.</param>
        /// <returns>The <see cref="OpenApiResult" />.</returns>
        [OperationId(SendTriggerOperationId)]
        public async Task<OpenApiResult> HandleTrigger(string tenantId, string workflowInstanceId, IWorkflowTrigger body)
        {
            ITenant tenant = await this.marainServicesTenancy.GetRequestingTenantAsync(tenantId).ConfigureAwait(false);
            IWorkflowEngine workflowEngine = await this.workflowEngineFactory.GetWorkflowEngineAsync(tenant).ConfigureAwait(false);
            await workflowEngine.ProcessTriggerAsync(body, workflowInstanceId).ConfigureAwait(false);
            return this.OkResult();
        }

        /// <summary>
        /// Handles requests to start new workflow instances.
        /// </summary>
        /// <param name="tenantId">The tenant Id for the current request.</param>
        /// <param name="body">The request body.</param>
        /// <returns>The <see cref="Task" />.</returns>
        [OperationId(StartWorkflowInstanceOperationId)]
        public async Task<OpenApiResult> StartWorkflowInstance(string tenantId, StartWorkflowInstanceRequest body)
        {
            ITenant tenant = await this.marainServicesTenancy.GetRequestingTenantAsync(tenantId).ConfigureAwait(false);
            IWorkflowEngine workflowEngine = await this.workflowEngineFactory.GetWorkflowEngineAsync(tenant).ConfigureAwait(false);
            await workflowEngine.StartWorkflowInstanceAsync(body).ConfigureAwait(false);
            return this.CreatedResult();
        }

        /// <summary>
        /// Gets a workflow definition.
        /// </summary>
        /// <param name="tenantId">The tenant Id for the current request.</param>
        /// <param name="workflowId">The workflow Id.</param>
        /// <returns>The <see cref="Task" />.</returns>
        [OperationId(GetWorkflowOperationId)]
        public async Task<OpenApiResult> GetWorkflow(string tenantId, string workflowId)
        {
            ITenant tenant = await this.marainServicesTenancy.GetRequestingTenantAsync(tenantId).ConfigureAwait(false);
            IWorkflowStore workflowStore = await this.workflowStoreFactory.GetWorkflowStoreForTenantAsync(tenant).ConfigureAwait(false);
            Workflow workflow = await workflowStore.GetWorkflowAsync(workflowId).ConfigureAwait(false);
            OpenApiResult result = this.OkResult(workflow, "application/json");

            if (!string.IsNullOrEmpty(workflow.ETag))
            {
                result.Results.Add("ETag", workflow.ETag);
            }

            return result;
        }

        /// <summary>
        /// Stores a workflow definition.
        /// </summary>
        /// <param name="tenantId">The tenant Id for the current request.</param>
        /// <param name="body">The workflow to store.</param>
        /// <returns>The <see cref="OpenApiResult"/> to return.</returns>
        [OperationId(CreateWorkflowOperationId)]
        public async Task<OpenApiResult> CreateWorkflow(string tenantId, Workflow body)
        {
            // New workflow definitions shouldn't contain an etag.
            if (!string.IsNullOrEmpty(body.ETag))
            {
                return new OpenApiResult { StatusCode = 400 };
            }

            ITenant tenant = await this.marainServicesTenancy.GetRequestingTenantAsync(tenantId).ConfigureAwait(false);
            IWorkflowStore workflowStore = await this.workflowStoreFactory.GetWorkflowStoreForTenantAsync(tenant).ConfigureAwait(false);

            await workflowStore.UpsertWorkflowAsync(body).ConfigureAwait(false);
            return this.CreatedResult();
        }

        /// <summary>
        /// Updates a workflow definition.
        /// </summary>
        /// <param name="tenantId">The tenant Id for the current request.</param>
        /// <param name="workflowId">The Id of the workflow to update.</param>
        /// <param name="ifMatch">The last known etag value for the workflow.</param>
        /// <param name="body">The workflow to update.</param>
        /// <returns>The <see cref="OpenApiResult"/> to return.</returns>
        [OperationId(UpdateWorkflowOperationId)]
        public async Task<OpenApiResult> UpdateWorkflow(string tenantId, string workflowId, string ifMatch, Workflow body)
        {
            // Workflow Id in the path should match that in the body.
            if (workflowId != body.Id)
            {
                return new OpenApiResult { StatusCode = 400 };
            }

            body.ETag = ifMatch;

            ITenant tenant = await this.marainServicesTenancy.GetRequestingTenantAsync(tenantId).ConfigureAwait(false);
            IWorkflowStore workflowStore = await this.workflowStoreFactory.GetWorkflowStoreForTenantAsync(tenant).ConfigureAwait(false);

            // Because there's an etag in the body, the workflow store will throw an exception if either the workflow
            // doesn't already exist, or if the stored version doesn't have a matching etag.
            await workflowStore.UpsertWorkflowAsync(body).ConfigureAwait(false);

            return this.OkResult();
        }
    }
}