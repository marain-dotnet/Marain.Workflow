// <copyright file="IWorkflowEngineFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System.Threading.Tasks;
    using Corvus.Azure.Cosmos.Tenancy;
    using Corvus.Tenancy;

    /// <summary>
    /// A factory for creating a workflow engine.
    /// </summary>
    public interface IWorkflowEngineFactory
    {
        /// <summary>
        /// Gets or sets the repository definition for the workflow instance repository.
        /// </summary>
        CosmosContainerDefinition WorkflowInstanceCosmosContainerDefinition { get; set; }

        /// <summary>
        /// Gets or sets the repository definition for the workflow repository.
        /// </summary>
        CosmosContainerDefinition WorkflowCosmosContainerDefinition { get; set; }

        /// <summary>
        /// Gets an <see cref="IWorkflowEngine"/> for the tenant.
        /// </summary>
        /// <param name="tenant">The tenant for which to get the workflow engine.</param>
        /// <returns>A <see cref="Task"/> which completes with the workflow engine for the tenant.</returns>
        Task<IWorkflowEngine> GetWorkflowEngineAsync(ITenant tenant);
    }
}
