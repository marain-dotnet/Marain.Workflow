// <copyright file="WorkflowEngineClientServiceCollectionExtensions.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using System.Linq;
    using Corvus.Extensions.Json;
    using Corvus.Identity.ManagedServiceIdentity.ClientAuthentication;
    using Marain.Workflows.Client;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Rest;

    /// <summary>
    /// DI initialization for clients of the workflow engine service.
    /// </summary>
    public static class WorkflowEngineClientServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the workflow engine client to a service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="baseUri">The base URI of the workflow engine service.</param>
        /// <param name="resourceIdForMsiAuthentication">
        /// The resource id to use when obtaining an authentication token representing the
        /// hosting service's identity. Pass null to run without authentication.
        /// </param>
        /// <returns>The modified service collection.</returns>
        public static IServiceCollection AddMarainWorkflowEngineClient(
            this IServiceCollection services,
            Uri baseUri,
            string resourceIdForMsiAuthentication = null)
        {
            if (services.Any(s => s.ServiceType == typeof(IMarainWorkflowEngine)))
            {
                return services;
            }

            return resourceIdForMsiAuthentication == null
                ? services.AddSingleton<IMarainWorkflowEngine>(new UnauthenticatedMarainWorkflowEngine(baseUri))
                : services.AddSingleton<IMarainWorkflowEngine>(sp =>
                {
                    var service = new MarainWorkflowEngine(baseUri, new TokenCredentials(
                        new ServiceIdentityTokenProvider(
                            sp.GetRequiredService<IServiceIdentityTokenSource>(),
                            resourceIdForMsiAuthentication)));

                    sp.GetRequiredService<IJsonSerializerSettingsProvider>().Instance.Converters.ForEach(service.SerializationSettings.Converters.Add);
                    sp.GetRequiredService<IJsonSerializerSettingsProvider>().Instance.Converters.ForEach(service.DeserializationSettings.Converters.Add);

                    return service;
                });
        }
    }
}
