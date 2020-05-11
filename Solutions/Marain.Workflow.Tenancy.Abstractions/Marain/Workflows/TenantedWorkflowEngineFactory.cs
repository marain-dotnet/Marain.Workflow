// <copyright file="TenantedWorkflowEngineFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System;
    using System.Threading.Tasks;
    using Corvus.Leasing;
    using Corvus.Tenancy;
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
        private readonly ITenantedWorkflowInstanceChangeLogFactory workflowInstanceChangeLogFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantedWorkflowEngineFactory"/> class.
        /// </summary>
        /// <param name="workflowStoreFactory">The factory for retrieving tenanted workflow stores.</param>
        /// <param name="workflowInstanceStoreFactory">The factory for retrieving tenanted workflow instance stores.</param>
        /// <param name="workflowInstanceChangeLogFactory">The factory for retrieving tenanted workflow instance change logs.</param>
        /// <param name="leaseProvider">The lease provider.</param>
        /// <param name="logger">The logger.</param>
        public TenantedWorkflowEngineFactory(
            ITenantedWorkflowStoreFactory workflowStoreFactory,
            ITenantedWorkflowInstanceStoreFactory workflowInstanceStoreFactory,
            ITenantedWorkflowInstanceChangeLogFactory workflowInstanceChangeLogFactory,
            ILeaseProvider leaseProvider,
            ILogger<IWorkflowEngine> logger)
        {
            this.leaseProvider = leaseProvider ?? throw new ArgumentNullException(nameof(leaseProvider));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.workflowStoreFactory = workflowStoreFactory ?? throw new ArgumentNullException(nameof(workflowStoreFactory));
            this.workflowInstanceStoreFactory = workflowInstanceStoreFactory ?? throw new ArgumentNullException(nameof(workflowInstanceStoreFactory));
            this.workflowInstanceChangeLogFactory = workflowInstanceChangeLogFactory ?? throw new ArgumentNullException(nameof(workflowInstanceChangeLogFactory));
        }

        /// <inheritdoc/>
        public async Task<IWorkflowEngine> GetWorkflowEngineAsync(ITenant tenant)
        {
            return new WorkflowEngine(
                await this.workflowStoreFactory.GetWorkflowStoreForTenantAsync(tenant).ConfigureAwait(false),
                await this.workflowInstanceStoreFactory.GetWorkflowInstanceStoreForTenantAsync(tenant).ConfigureAwait(false),
                await this.workflowInstanceChangeLogFactory.GetWorkflowInstanceChangeLogForTenantAsync(tenant).ConfigureAwait(false),
                this.leaseProvider,
                this.logger);
        }
    }
}
