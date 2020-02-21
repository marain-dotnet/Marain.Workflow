// <copyright file="WorkflowEngineClientServiceCollectionExtensions.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using System.Linq;
    using Corvus.Extensions.Json;
    using Corvus.Identity.ManagedServiceIdentity.ClientAuthentication;
    using Marain.Workflows.MessageHost.Client;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Rest;

    /// <summary>
    /// DI initialization for clients of the workflow engine service.
    /// </summary>
    public static class WorkflowMessageIngestionClientServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the workflow engine client to a service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The modified service collection.</returns>
        public static IServiceCollection AddMarainWorkflowMessageIngestionClient(
            this IServiceCollection services,
            Func<IServiceProvider, MarainWorkflowMessageIngestionClientOptions> getOptions)
        {
            if (services.Any(s => s.ServiceType == typeof(IMarainWorkflowMessageIngestion)))
            {
                return services;
            }

            services.AddSingleton<IMarainWorkflowMessageIngestion>(sp =>
            {
                MarainWorkflowMessageIngestionClientOptions options = getOptions(sp);

                if (string.IsNullOrEmpty(options.ResourceIdForAuthentication))
                {
                    return new UnauthenticatedMarainWorkflowMessageIngestion(options.BaseUrl);
                }

                var service = new MarainWorkflowMessageIngestion(options.BaseUrl, new TokenCredentials(
                    new ServiceIdentityTokenProvider(
                        sp.GetRequiredService<IServiceIdentityTokenSource>(),
                        options.ResourceIdForAuthentication)));

                sp.GetRequiredService<IJsonSerializerSettingsProvider>().Instance.Converters.ForEach(service.SerializationSettings.Converters.Add);

                return service;
            });

            return services;
        }
    }
}
