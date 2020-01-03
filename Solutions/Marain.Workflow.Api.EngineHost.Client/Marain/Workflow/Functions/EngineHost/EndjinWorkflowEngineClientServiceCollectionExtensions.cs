// <copyright file="EndjinWorkflowEngineClientServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflow.Functions.EngineHost
{
    using System;
    using Marain.Identity.ManagedServiceIdentity.ClientAuthentication;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Rest;

    /// <summary>
    /// DI initialization for clients of the Workflow service.
    /// </summary>
    public static class EndjinWorkflowEngineClientServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the Workflow Engine client to a service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="baseUri">The base URI of the Workflow Engine service.</param>
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
                ? services.AddSingleton<IEndjinWorkflowEngine>(new UnauthenticatedEndjinWorkflowEngine(baseUri))
                : services.AddSingleton<IEndjinWorkflowEngine>(sp =>
                    new EndjinWorkflowEngine(
                        baseUri,
                        new TokenCredentials(
                            new ServiceIdentityTokenProvider(
                                sp.GetRequiredService<IServiceIdentityTokenSource>(),
                                resourceIdForMsiAuthentication))));
        }
    }
}
