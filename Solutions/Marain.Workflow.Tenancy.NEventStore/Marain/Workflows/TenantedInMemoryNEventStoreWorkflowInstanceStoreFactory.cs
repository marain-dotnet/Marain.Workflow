// <copyright file="TenantedInMemoryNEventStoreWorkflowInstanceStoreFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using Corvus.Tenancy;
    using Marain.Workflows.Internal;
    using Marain.Workflows.Storage;
    using Microsoft.Extensions.Logging;
    using NEventStore;
    using NEventStore.Serialization.Json;

    /// <summary>
    /// Factory class for retrieving in-memory event store instances of <see cref="IWorkflowInstanceStore"/> for specific <see cref="Tenant"/>s.
    /// </summary>
    public class TenantedInMemoryNEventStoreWorkflowInstanceStoreFactory : ITenantedWorkflowInstanceStoreFactory
    {
        private readonly ConcurrentDictionary<string, Task<IStoreEvents>> eventStores = new ConcurrentDictionary<string, Task<IStoreEvents>>();
        private readonly ILogger<NEventStoreWorkflowInstanceStore> storeLogger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantedInMemoryNEventStoreWorkflowInstanceStoreFactory"/> class.
        /// </summary>
        /// <param name="storeLogger">The logger for the workflow instance store.</param>
        public TenantedInMemoryNEventStoreWorkflowInstanceStoreFactory(
            ILogger<NEventStoreWorkflowInstanceStore> storeLogger)
        {
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
                _ => this.BuildStore()).ConfigureAwait(false);

            return new NEventStoreWorkflowInstanceStore(store, this.storeLogger);
        }

        private Task<IStoreEvents> BuildStore()
        {
            IStoreEvents result = Wireup.Init()
                .UsingInMemoryPersistence()
                .UsingJsonSerialization()
                .Build();

            return Task.FromResult(result);
        }
    }
}
