// <copyright file="MarainWorkflowServiceServiceCollectionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.DependencyInjection
{
    using System;
    using System.Linq;

    using Corvus.Extensions.Json;
    using Corvus.Identity.ClientAuthentication.MicrosoftRest;

    using Marain.Workflows.Api.Client;
    using Microsoft.Rest;

    using Newtonsoft.Json;

    /// <summary>
    /// DI initialization for clients of the workflow message ingestion service.
    /// </summary>
    public static class MarainWorkflowServiceServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the workflow message ingestion client to a service collection.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <param name="getOptions">Callback function to provide workflow options for Marain.</param>
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

                var tokenCredentials = new TokenCredentials(
                        sp.GetRequiredService<IServiceIdentityMicrosoftRestTokenProviderSource>().GetTokenProvider(
                            $"{options.ResourceIdForAuthentication}/.default"));
                var service = new MarainWorkflowService(options.BaseUrl, tokenCredentials);

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