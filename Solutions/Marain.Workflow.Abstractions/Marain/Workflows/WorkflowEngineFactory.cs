// <copyright file="WorkflowEngineFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System.Threading.Tasks;
    using Corvus.Azure.Cosmos.Tenancy;
    using Corvus.Leasing;
    using Corvus.Tenancy;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// A standard workflow engine factory.
    /// </summary>
    public class WorkflowEngineFactory : IWorkflowEngineFactory
    {
        private readonly ITenantCosmosContainerFactory repositoryFactory;
        private readonly ILeaseProvider leaseProvider;
        private readonly ILogger<IWorkflowEngine> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowEngineFactory"/> class.
        /// </summary>
        /// <param name="repositoryFactory">The <see cref="ITenantCosmosContainerFactory"/>.</param>
        /// <param name="leaseProvider">The lease provider.</param>
        /// <param name="logger">The logger.</param>
        public WorkflowEngineFactory(
            ITenantCosmosContainerFactory repositoryFactory,
            ILeaseProvider leaseProvider,
            ILogger<IWorkflowEngine> logger)
        {
            this.repositoryFactory = repositoryFactory;
            this.leaseProvider = leaseProvider;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public CosmosContainerDefinition WorkflowInstanceCosmosContainerDefinition { get; set; } =
            new CosmosContainerDefinition("workflow", "workflowinstances", "/id");

        /// <inheritdoc/>
        public CosmosContainerDefinition WorkflowCosmosContainerDefinition { get; set; } =
            new CosmosContainerDefinition("workflow", "workflows", "/id");

        /// <inheritdoc/>
        public async Task<IWorkflowEngine> GetWorkflowEngineAsync(ITenant tenant)
        {
            Container workflowInstanceRepository = await this.repositoryFactory.GetContainerForTenantAsync(
                tenant,
                this.WorkflowInstanceCosmosContainerDefinition).ConfigureAwait(false);

            Container workflowRepository = await this.repositoryFactory.GetContainerForTenantAsync(
                tenant,
                this.WorkflowCosmosContainerDefinition).ConfigureAwait(false);

            return new WorkflowEngine(workflowInstanceRepository, workflowRepository, this.leaseProvider, this.logger);
        }
    }
}
