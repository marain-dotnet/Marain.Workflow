﻿// <copyright file="WorkflowServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using Corvus.Azure.Cosmos.Tenancy;
    using Corvus.Leasing;
    using Marain.Workflows;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Service collection extensions to add workflow event hub trigger queueing.
    /// </summary>
    public static class WorkflowServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the standard <see cref="IWorkflowEngineFactory"/> implementation to the given
        /// <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="collection">The Service Collection to add to.</param>
        /// <param name="configureFactory">Configure the <see cref="IWorkflowEngineFactory"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/>.</returns>
        /// <remarks>
        /// The default workflow engine has dependencies on
        /// - <see cref="ITenantCosmosContainerFactory"/>
        /// - <see cref="ILeaseProvider"/>
        /// - <see cref="ILogger"/>
        /// Implementations of these services must be added to the service collection separately.
        /// </remarks>
        public static IServiceCollection AddWorkflowEngineFactory(
            this IServiceCollection collection,
            Action<IWorkflowEngineFactory> configureFactory = null)
        {
            collection.AddSingleton<IWorkflowEngineFactory>(s =>
            {
                var result = new WorkflowEngineFactory(
                    s.GetRequiredService<ITenantCosmosContainerFactory>(),
                    s.GetRequiredService<ILeaseProvider>(),
                    s.GetRequiredService<ILogger<IWorkflowEngine>>());

                configureFactory?.Invoke(result);
                return result;
            });

            return collection;
        }
    }
}