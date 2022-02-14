// <copyright file="WorkflowEngineClientServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using System.Linq;

    using Corvus.Extensions.Json;
    using Corvus.Identity.ClientAuthentication.MicrosoftRest;

    using Marain.Workflows.EngineHost.Client;

    using Microsoft.Rest;

    using Newtonsoft.Json;

    /// <summary>
    /// DI initialization for clients of the workflow engine service.
    /// </summary>
    public static class WorkflowEngineClientServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the workflow engine client to a service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="getOptions">A callback method to retrieve options for the client.</param>
        /// <returns>The modified service collection.</returns>
        public static IServiceCollection AddMarainWorkflowEngineClient(
            this IServiceCollection services,
            Func<IServiceProvider, MarainWorkflowEngineClientOptions> getOptions)
        {
            return services
                .AddSingleton(sp => getOptions(sp))
                .AddMarainWorkflowEngineClient();
        }

        /// <summary>
        /// Adds the workflow engine client to a service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The modified service collection.</returns>
        public static IServiceCollection AddMarainWorkflowEngineClient(this IServiceCollection services)
        {
            if (services.Any(s => s.ServiceType == typeof(IMarainWorkflowEngine)))
            {
                return services;
            }

            services.AddSingleton<IMarainWorkflowEngine>(sp =>
            {
                MarainWorkflowEngineClientOptions options = sp.GetRequiredService<MarainWorkflowEngineClientOptions>();

                if (string.IsNullOrEmpty(options.ResourceIdForAuthentication))
                {
                    return new UnauthenticatedMarainWorkflowEngine(options.BaseUrl);
                }

                var tokenCredentials = new TokenCredentials(
                        sp.GetRequiredService<IServiceIdentityMicrosoftRestTokenProviderSource>().GetTokenProvider(
                            $"{options.ResourceIdForAuthentication}/.default"));
                var service = new MarainWorkflowEngine(options.BaseUrl, tokenCredentials);

                foreach (JsonConverter converter in sp.GetRequiredService<IJsonSerializerSettingsProvider>().Instance.Converters)
                {
                    service.SerializationSettings.Converters.Add(converter);
                }

                return service;
            });

            return services;
        }
    }
}