// <copyright file="ITenantedWorkflowEngineFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System.Threading.Tasks;
    using Corvus.Tenancy;

    /// <summary>
    /// A factory for creating a workflow engine.
    /// </summary>
    public interface ITenantedWorkflowEngineFactory
    {
        /// <summary>
        /// Gets an <see cref="IWorkflowEngine"/> for the tenant.
        /// </summary>
        /// <param name="tenant">The tenant for which to get the workflow engine.</param>
        /// <returns>A <see cref="Task"/> which completes with the workflow engine for the tenant.</returns>
        Task<IWorkflowEngine> GetWorkflowEngineAsync(ITenant tenant);
    }
}