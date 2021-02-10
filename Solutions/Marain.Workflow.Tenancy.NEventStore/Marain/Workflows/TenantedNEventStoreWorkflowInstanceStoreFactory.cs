// <copyright file="TenantedNEventStoreWorkflowInstanceStoreFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using Corvus.Azure.Cosmos.Tenancy;
    using Corvus.Tenancy;
    using Marain.Workflows.Storage;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Extensions.Logging;
    using NEventStore;
    using NEventStore.Persistence.CosmosDb;
    using NEventStore.Serialization.Json;

    /// <summary>
    /// Factory class for retrieving Cosmos-backed event store instances of <see cref="IWorkflowInstanceStore"/> for specific <see cref="Tenant"/>s.
    /// </summary>
    public class TenantedNEventStoreWorkflowInstanceStoreFactory : ITenantedWorkflowInstanceStoreFactory
    {
        private readonly ILogger<NEventStoreWorkflowInstanceStore> storeLogger;
        private readonly ConcurrentDictionary<string, Task<IStoreEvents>> eventStores = new ConcurrentDictionary<string, Task<IStoreEvents>>();
        private readonly ITenantedNEventStoreFactory eventStoreFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantedNEventStoreWorkflowInstanceStoreFactory"/> class.
        /// </summary>
        /// <param name="eventStoreFactory">The <see cref="ITenantedNEventStoreFactory"/> that will be used to create
        /// underlying <see cref="IStoreEvents"/> instances for the content stores.</param>
        /// <param name="storeLogger">The logger for the workflow instance store.</param>
        public TenantedNEventStoreWorkflowInstanceStoreFactory(
            ITenantedNEventStoreFactory eventStoreFactory,
            ILogger<NEventStoreWorkflowInstanceStore> storeLogger)
        {
            this.eventStoreFactory = eventStoreFactory
                ?? throw new ArgumentNullException(nameof(eventStoreFactory));

            this.storeLogger = storeLogger
                ?? throw new ArgumentNullException(nameof(storeLogger));
        }

        /// <inheritdoc/>
        public async Task<IWorkflowInstanceStore> GetWorkflowInstanceStoreForTenantAsync(ITenant tenant)
        {
            if (tenant is null)
            {
                throw new ArgumentNullException(nameof(tenant));
            }

            IStoreEvents store = await this.eventStores.GetOrAdd(
                tenant.Id,
                _ => this.eventStoreFactory.GetStoreForTenantAsync(tenant)).ConfigureAwait(false);

            return new NEventStoreWorkflowInstanceStore(store, this.storeLogger);
        }
    }
}
