// <copyright file="AzureWorkflowServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using Marain.Workflows;
    using Marain.Workflows.Internal;

    /// <summary>
    /// Service collection extensions to add workflow event hub trigger queueing.
    /// </summary>
    public static class AzureWorkflowServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the event hub implementation of <see cref="IWorkflowMessageQueue"/>
        /// to the given <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="collection">
        /// The Service Collection to add to.
        /// </param>
        /// <returns>
        /// The <see cref="IServiceCollection"/>.
        /// </returns>
        public static IServiceCollection AddAzureEventHubWorkflowTriggerQueue(this IServiceCollection collection)
        {
            collection.AddSingleton<IWorkflowMessageQueue, EventHubWorkflowMessageQueue>();

            return collection;
        }
    }
}