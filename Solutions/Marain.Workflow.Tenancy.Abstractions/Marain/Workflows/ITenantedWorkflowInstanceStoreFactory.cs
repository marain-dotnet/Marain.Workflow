// <copyright file="ITenantedWorkflowInstanceStoreFactory.cs" company="Endjin Limited">
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
    public interface ITenantedWorkflowInstanceStoreFactory
    {
        /// <summary>
        /// Retrieves an <see cref="IWorkflowInstanceStore"/> for the specified <see cref="Tenant"/>.
        /// </summary>
        /// <param name="tenant">The tenant to retrieve a workflow store for.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<IWorkflowInstanceStore> GetWorkflowInstanceStoreForTenantAsync(ITenant tenant);
    }
}