﻿// <copyright file="EngineService.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.Services
{
    using System.Threading.Tasks;
    using Corvus.Tenancy;
    using Marain.Services.Tenancy;
    using Menes;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Handles incoming triggers posted to the trigger service.
    /// </summary>
    [EmbeddedOpenApiDefinition("Marain.Workflows.Api.Services.EngineService.yaml")]
    public class EngineService : IOpenApiService
    {
        private const string StartWorkflowInstanceOperationId = "startWorkflowInstance";
        private const string SendTriggerOperationId = "sendTrigger";

        private readonly IMarainServicesTenancy marainServicesTenancy;
        private readonly ITenantedWorkflowEngineFactory workflowEngineFactory;
        private readonly IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="EngineService"/> class.
        /// </summary>
        /// <param name="workflowEngineFactory">The workflow engine factory.</param>
        /// <param name="marainServicesTenancy">The tenancy services.</param>
        /// <param name="configuration">The configuration.</param>
        public EngineService(
            ITenantedWorkflowEngineFactory workflowEngineFactory,
            IMarainServicesTenancy marainServicesTenancy,
            IConfiguration configuration)
        {
            this.workflowEngineFactory = workflowEngineFactory;
            this.marainServicesTenancy = marainServicesTenancy;
            this.configuration = configuration;
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
    }
}