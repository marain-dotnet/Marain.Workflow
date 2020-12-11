// <copyright file="ITenantedCloudEventPublisherFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System.Threading.Tasks;
    using Corvus.Tenancy;
    using Marain.Workflows.CloudEvents;

    /// <summary>
    /// A factory for creating a cloud event publisher.
    /// </summary>
    public interface ITenantedCloudEventPublisherFactory
    {
        /// <summary>
        /// Gets an <see cref="IWorkflowEngine"/> for the tenant.
        /// </summary>
        /// <param name="tenant">The tenant for which to get the workflow engine.</param>
        /// <returns>A <see cref="Task"/> which completes with the workflow engine for the tenant.</returns>
        Task<CloudEventPublisher> GetCloudEventPublisherAsync(ITenant tenant);
    }
}
