// <copyright file="WorkflowClientServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using Marain.Identity.ManagedServiceIdentity.ClientAuthentication;
    using Marain.Workflow.Client;
    using Microsoft.Rest;

    /// <summary>
    /// DI service configuration for the Workflow client.
    /// </summary>
    public static class WorkflowClientServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the Workflow public API client to a service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="baseUri">The base URI of the Workflow Message Ingestion service.</param>
        /// <param name="resourceIdForMsiAuthentication">
        /// The resource id to use when obtaining an authentication token representing the
        /// hosting service's identity. Pass null to run without authentication.
        /// </param>
        /// <returns>The modified service collection.</returns>
        public static IServiceCollection AddEndjinWorkflowEngineClient(
            this IServiceCollection services,
            Uri baseUri,
            string resourceIdForMsiAuthentication = null)
        {
            return resourceIdForMsiAuthentication == null
                ? services.AddSingleton<IWorkflowService>(new UnauthenticatedWorkflowService(baseUri))
                : services.AddSingleton<IWorkflowService>(sp =>
                    new WorkflowService(
                        baseUri,
                        new TokenCredentials(
                            new ServiceIdentityTokenProvider(
                                sp.GetRequiredService<IServiceIdentityTokenSource>(),
                                resourceIdForMsiAuthentication))));
        }
    }
}
