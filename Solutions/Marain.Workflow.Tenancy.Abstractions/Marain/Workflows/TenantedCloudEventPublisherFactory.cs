// <copyright file="TenantedCloudEventPublisherFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System.Net.Http;
    using System.Threading.Tasks;
    using Corvus.Extensions.Json;
    using Corvus.Identity.ManagedServiceIdentity.ClientAuthentication;
    using Corvus.Tenancy;
    using Marain.Workflows.CloudEvents;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Standard implementation of the <see cref="ITenantedCloudEventPublisherFactory"/>.
    /// </summary>
    public class TenantedCloudEventPublisherFactory : ITenantedCloudEventPublisherFactory
    {
        private readonly HttpClient httpClient;
        private readonly IServiceIdentityTokenSource serviceIdentityTokenSource;
        private readonly IJsonSerializerSettingsProvider serializerSettingsProvider;
        private readonly ILogger<CloudEventPublisher> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudEventPublisher"/> class.
        /// </summary>
        /// <param name="httpClient">The <see cref="HttpClient"/> to use when POSTing event data to subscribers.</param>
        /// <param name="serviceIdentityTokenSource">The <see cref="IServiceIdentityTokenSource"/> that will be used to aquire authentication tokens.</param>
        /// <param name="serializerSettingsProvider">The current <see cref="IJsonSerializerSettingsProvider"/>.</param>
        /// <param name="logger">The logger.</param>
        public TenantedCloudEventPublisherFactory(
            HttpClient httpClient,
            IServiceIdentityTokenSource serviceIdentityTokenSource,
            IJsonSerializerSettingsProvider serializerSettingsProvider,
            ILogger<CloudEventPublisher> logger)
        {
            this.httpClient = httpClient;
            this.serviceIdentityTokenSource = serviceIdentityTokenSource;
            this.serializerSettingsProvider = serializerSettingsProvider;
            this.logger = logger;
        }

        /// <inheritdoc/>
        public Task<CloudEventPublisher> GetCloudEventPublisherAsync(ITenant tenant)
        {
            return Task.FromResult(
                new CloudEventPublisher(
                    tenant.Id,
                    this.httpClient,
                    this.serviceIdentityTokenSource,
                    this.serializerSettingsProvider,
                    this.logger));
        }
    }
}