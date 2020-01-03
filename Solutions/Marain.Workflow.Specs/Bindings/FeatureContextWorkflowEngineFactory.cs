// <copyright file="FeatureContextWorkflowEngineFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Specs.Bindings
{
    using System.Threading.Tasks;
    using Corvus.Azure.Cosmos.Tenancy;
    using Corvus.Leasing;
    using Corvus.Tenancy;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Extensions.Logging;
    using TechTalk.SpecFlow;

    /// <summary>
    /// The feature context workflow engine factory.
    /// </summary>
    public class FeatureContextWorkflowEngineFactory : IWorkflowEngineFactory
    {
        private readonly FeatureContext featureContext;
        private readonly ILeaseProvider leaseProvider;
        private readonly ILogger<IWorkflowEngine> logger;

        /// <summary>
        /// Creates an instance of the FeatureContextWorkflowRepositoriesFactory for a specific FeatureContext.
        /// </summary>
        /// <param name="featureContext">The feature context.</param>
        /// <param name="leaseProvider">The lease provider.</param>
        /// <param name="logger">The logger.</param>
        public FeatureContextWorkflowEngineFactory(
            FeatureContext featureContext,
            ILeaseProvider leaseProvider,
            ILogger<IWorkflowEngine> logger)
        {
            this.featureContext = featureContext;
            this.leaseProvider = leaseProvider;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public CosmosContainerDefinition WorkflowInstanceCosmosContainerDefinition { get; set; }

        /// <inheritdoc/>
        public CosmosContainerDefinition WorkflowCosmosContainerDefinition { get; set; }

        /// <inheritdoc/>
        public Task<IWorkflowEngine> GetWorkflowEngineAsync(ITenant tenant)
        {
            return Task.FromResult(
                (IWorkflowEngine)new WorkflowEngine(
                    (Container)this.featureContext[WorkflowCosmosDbBindings.WorkflowInstancesRepository],
                    (Container)this.featureContext[WorkflowCosmosDbBindings.WorkflowsRepository],
                    this.leaseProvider,
                    this.logger));
        }
    }
}