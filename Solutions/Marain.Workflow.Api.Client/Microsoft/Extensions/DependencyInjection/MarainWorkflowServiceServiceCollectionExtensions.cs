// <copyright file="WorkflowEngineClientServiceCollectionExtensions.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using System.Linq;
    using Corvus.Extensions.Json;
    using Corvus.Identity.ManagedServiceIdentity.ClientAuthentication;
    using Marain.Workflows.Api.Client;
    using Microsoft.Rest;

    /// <summary>
    /// DI initialization for clients of the workflow message ingestion service.
    /// </summary>
    public static class MarainWorkflowServiceServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the workflow message ingestion client to a service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The modified service collection.</returns>
        public static IServiceCollection AddMarainWorkflowMessageIngestionClient(
            this IServiceCollection services,
            Func<IServiceProvider, MarainWorkflowServiceOptions> getOptions)
        {
            if (services.Any(s => s.ServiceType == typeof(IMarainWorkflowService)))
            {
                return services;
            }

            services.AddSingleton<IMarainWorkflowService>(sp =>
            {
                MarainWorkflowServiceOptions options = getOptions(sp);

                if (string.IsNullOrEmpty(options.ResourceIdForAuthentication))
                {
                    return new UnauthenticatedMarainWorkflowService(options.BaseUrl);
                }

                var service = new MarainWorkflowService(options.BaseUrl, new TokenCredentials(
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
