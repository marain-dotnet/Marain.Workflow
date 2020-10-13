// <copyright file="TenantedWorkflowEngineFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System;
    using System.Threading.Tasks;
    using Corvus.Leasing;
    using Corvus.Tenancy;
    using Marain.Workflows.CloudEvents;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// A standard workflow engine factory.
    /// </summary>
    public class TenantedWorkflowEngineFactory : ITenantedWorkflowEngineFactory
    {
        private readonly ILeaseProvider leaseProvider;
        private readonly ILogger<IWorkflowEngine> logger;
        private readonly ITenantedWorkflowStoreFactory workflowStoreFactory;
        private readonly ITenantedWorkflowInstanceStoreFactory workflowInstanceStoreFactory;
        private readonly ICloudEventDataPublisher cloudEventPublisher;
        private readonly TenantedWorkflowEngineFactoryConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantedWorkflowEngineFactory"/> class.
        /// </summary>
        /// <param name="configuration">Configuration containing the base cloud event publisher source that will be used to create a tenant-specific source for the workflow engine.</param>
        /// <param name="workflowStoreFactory">The factory for retrieving tenanted workflow stores.</param>
        /// <param name="workflowInstanceStoreFactory">The factory for retrieving tenanted workflow instance stores.</param>
        /// <param name="leaseProvider">The lease provider.</param>
        /// <param name="cloudEventPublisher">The publisher for workflow events.</param>
        /// <param name="logger">The logger.</param>
        public TenantedWorkflowEngineFactory(
            TenantedWorkflowEngineFactoryConfiguration configuration,
            ITenantedWorkflowStoreFactory workflowStoreFactory,
            ITenantedWorkflowInstanceStoreFactory workflowInstanceStoreFactory,
            ILeaseProvider leaseProvider,
            ICloudEventDataPublisher cloudEventPublisher,
            ILogger<IWorkflowEngine> logger)
        {
            this.leaseProvider = leaseProvider ?? throw new ArgumentNullException(nameof(leaseProvider));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.workflowStoreFactory = workflowStoreFactory ?? throw new ArgumentNullException(nameof(workflowStoreFactory));
            this.workflowInstanceStoreFactory = workflowInstanceStoreFactory ?? throw new ArgumentNullException(nameof(workflowInstanceStoreFactory));
            this.cloudEventPublisher = cloudEventPublisher ?? throw new ArgumentNullException(nameof(cloudEventPublisher));
            this.configuration = configuration;
        }

        /// <inheritdoc/>
        public async Task<IWorkflowEngine> GetWorkflowEngineAsync(ITenant tenant)
        {
            return new WorkflowEngine(
                await this.workflowStoreFactory.GetWorkflowStoreForTenantAsync(tenant).ConfigureAwait(false),
                await this.workflowInstanceStoreFactory.GetWorkflowInstanceStoreForTenantAsync(tenant).ConfigureAwait(false),
                this.leaseProvider,
                $"{this.configuration.CloudEventBaseSourceName}.{tenant.Id}",
                this.cloudEventPublisher,
                this.logger);
        }
    }
}
