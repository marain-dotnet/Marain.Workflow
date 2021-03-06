﻿// <copyright file="WorkflowEngineClientServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using System.Linq;
    using Corvus.Extensions.Json;
    using Corvus.Identity.ManagedServiceIdentity.ClientAuthentication;
    using Marain.Workflows.EngineHost.Client;
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
        /// <param name="getOptions">A callback method to retrieve options for the client.</param>
        /// <returns>The modified service collection.</returns>
        public static IServiceCollection AddMarainWorkflowEngineClient(
            this IServiceCollection services,
            Func<IServiceProvider, MarainWorkflowEngineClientOptions> getOptions)
        {
            if (services.Any(s => s.ServiceType == typeof(IMarainWorkflowEngine)))
            {
                return services;
            }

            services.AddSingleton<IMarainWorkflowEngine>(sp =>
            {
                MarainWorkflowEngineClientOptions options = getOptions(sp);

                if (string.IsNullOrEmpty(options.ResourceIdForAuthentication))
                {
                    return new UnauthenticatedMarainWorkflowEngine(options.BaseUrl);
                }

                var service = new MarainWorkflowEngine(options.BaseUrl, new TokenCredentials(
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
