// <copyright file="ITenantedWorkflowInstanceChangeLogFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System.Threading.Tasks;
    using Corvus.Tenancy;

    /// <summary>
    /// Interface for a factory that knows how to provide a workflow store for
    /// a specific tenant.
    /// </summary>
    public interface ITenantedWorkflowInstanceChangeLogFactory
    {
        /// <summary>
        /// Retrieves an <see cref="IWorkflowInstanceChangeLog"/> for the specified <see cref="Tenant"/>.
        /// </summary>
        /// <param name="tenant">The tenant for which to retrieve a workflow instance change log.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<IWorkflowInstanceChangeLog> GetWorkflowInstanceChangeLogForTenantAsync(ITenant tenant);
    }
}
