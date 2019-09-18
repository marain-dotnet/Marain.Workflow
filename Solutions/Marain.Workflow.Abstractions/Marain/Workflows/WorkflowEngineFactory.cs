// <copyright file="WorkflowEngineFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System.Threading.Tasks;
    using Corvus.Azure.Cosmos.Tenancy;
    using Corvus.Extensions.Cosmos;
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
        /// <param name="repositoryFactory">The <see cref="ICosmosClientBuilderFactory"/>.</param>
        /// <param name="leaseProvider">The lease provider.</param>
        /// <param name="logger">The logger.</param>
        public WorkflowEngineFactory(
            ITenantCosmosContainerFactory repositoryFactory,
            ILeaseProvider leaseProvider,
            ILogger<IWorkflowEngine> logger)
        {
            this.repositoryFactory = repositoryFactory ?? throw new System.ArgumentNullException(nameof(repositoryFactory));
            this.leaseProvider = leaseProvider ?? throw new System.ArgumentNullException(nameof(leaseProvider));
            this.logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public CosmosContainerDefinition WorkflowInstanceRepositoryDefinition { get; set; } = new CosmosContainerDefinition("workflow", "workflowinstances", "/partitionKey");

        /// <inheritdoc/>
        public CosmosContainerDefinition WorkflowRepositoryDefinition { get; set; } = new CosmosContainerDefinition("workflow", "workflows", "/partitionKey");

        /// <inheritdoc/>
        public async Task<IWorkflowEngine> GetWorkflowEngineAsync(ITenant tenant)
        {
            Container workflowInstanceRepository = await this.repositoryFactory.GetContainerForTenantAsync(tenant, this.WorkflowInstanceRepositoryDefinition).ConfigureAwait(false);
            Container workflowRepository = await this.repositoryFactory.GetContainerForTenantAsync(tenant, this.WorkflowRepositoryDefinition).ConfigureAwait(false);

            return new WorkflowEngine(workflowInstanceRepository, workflowRepository, this.leaseProvider, this.logger);
        }
    }
}
