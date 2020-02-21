// <copyright file="TenantedWorkflowServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using Corvus.Leasing;
    using Marain.Workflows;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Service collection extensions to add workflow event hub trigger queueing.
    /// </summary>
    public static class TenantedWorkflowServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the standard <see cref="ITenantedWorkflowEngineFactory"/> implementation to the given
        /// <see cref="IServiceCollection"/>. Tenanted storage needs to be added separately.
        /// </summary>
        /// <param name="collection">The Service Collection to add to.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        /// <remarks>
        /// The default workflow engine has dependencies on
        /// - <see cref="ITenantedWorkflowStoreFactory"/>
        /// - <see cref="ITenantedWorkflowInstanceStoreFactory"/>
        /// - <see cref="ILeaseProvider"/>
        /// - <see cref="ILogger"/>
        /// Implementations of these services must be added to the service collection separately.
        /// </remarks>
        public static IServiceCollection AddTenantedWorkflowEngineFactory(this IServiceCollection collection)
        {
            collection.AddSingleton<ITenantedWorkflowEngineFactory, TenantedWorkflowEngineFactory>();

            return collection;
        }
    }
}