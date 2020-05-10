// <copyright file="TenantedWorkflowEngineFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System;
    using System.Threading.Tasks;
    using Corvus.Json;
    using Corvus.Leasing;
    using Corvus.Tenancy;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// A standard workflow engine factory.
    /// </summary>
    public class TenantedWorkflowEngineFactory : ITenantedWorkflowEngineFactory
    {
        private readonly ILeaseProvider leaseProvider;
        private readonly IPropertyBagFactory propertyBagFactory;
        private readonly ILogger<IWorkflowEngine> logger;
        private readonly ITenantedWorkflowStoreFactory workflowStoreFactory;
        private readonly ITenantedWorkflowInstanceStoreFactory workflowInstanceStoreFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantedWorkflowEngineFactory"/> class.
        /// </summary>
        /// <param name="workflowStoreFactory">The factory for retrieving tenanted workflow stores.</param>
        /// <param name="workflowInstanceStoreFactory">The factory for retrieving tenanted workflow instance stores.</param>
        /// <param name="leaseProvider">The lease provider.</param>
        /// <param name="propertyBagFactory">The <see cref="IPropertyBag"/>"/> factory.</param>
        /// <param name="logger">The logger.</param>
        public TenantedWorkflowEngineFactory(
            ITenantedWorkflowStoreFactory workflowStoreFactory,
            ITenantedWorkflowInstanceStoreFactory workflowInstanceStoreFactory,
            ILeaseProvider leaseProvider,
            IPropertyBagFactory propertyBagFactory,
            ILogger<IWorkflowEngine> logger)
        {
            this.leaseProvider = leaseProvider ?? throw new ArgumentNullException(nameof(leaseProvider));
            this.propertyBagFactory = propertyBagFactory ?? throw new ArgumentNullException(nameof(propertyBagFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.workflowStoreFactory = workflowStoreFactory ?? throw new ArgumentNullException(nameof(workflowStoreFactory));
            this.workflowInstanceStoreFactory = workflowInstanceStoreFactory ?? throw new ArgumentNullException(nameof(workflowInstanceStoreFactory));
        }

        /// <inheritdoc/>
        public async Task<IWorkflowEngine> GetWorkflowEngineAsync(ITenant tenant)
        {
            return new WorkflowEngine(
                await this.workflowStoreFactory.GetWorkflowStoreForTenantAsync(tenant).ConfigureAwait(false),
                await this.workflowInstanceStoreFactory.GetWorkflowInstanceStoreForTenantAsync(tenant).ConfigureAwait(false),
                this.leaseProvider,
                this.propertyBagFactory,
                this.logger);
        }
    }
}
