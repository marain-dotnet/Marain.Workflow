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
        /// <param name="resourceIdForMsiAuthentication">
        /// The resource id to use when obtaining an authentication token representing the
        /// hosting service's identity. Pass null to run without authentication.
        /// </param>
        /// <returns>The modified service collection.</returns>
        public static IServiceCollection AddMarainWorkflowEngineClient(
            this IServiceCollection services,
            Func<IServiceProvider, MarainWorkflowEngineOptions> getOptions,
            string resourceIdForMsiAuthentication = null)
        {
            if (services.Any(s => s.ServiceType == typeof(IMarainWorkflowEngine)))
            {
                return services;
            }

            return resourceIdForMsiAuthentication == null
                ? services.AddSingleton<IMarainWorkflowEngine>(sp =>
                {
                    MarainWorkflowEngineOptions options = getOptions(sp);
                    var service  = new UnauthenticatedMarainWorkflowEngine(options.BaseUri);

                    return service;
                })
                : services.AddSingleton<IMarainWorkflowEngine>(sp =>
                {
                    MarainWorkflowEngineOptions options = getOptions(sp);
                    var service = new MarainWorkflowEngine(options.BaseUri, new TokenCredentials(
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
