// <copyright file="TenantedCosmosDbNEventStoreWorkflowInstanceStoreFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;
    using Corvus.Azure.Cosmos.Tenancy;
    using Corvus.Tenancy;
    using Marain.Workflows.Internal;
    using Marain.Workflows.Storage;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Extensions.Logging;
    using NEventStore;
    using NEventStore.Persistence.CosmosDb;
    using NEventStore.Serialization.Json;

    /// <summary>
    /// Factory class for retrieving Cosmos-backed event store instances of <see cref="IWorkflowInstanceStore"/> for specific <see cref="Tenant"/>s.
    /// </summary>
    public class TenantedCosmosDbNEventStoreWorkflowInstanceStoreFactory : ITenantedWorkflowInstanceStoreFactory
    {
        /// <summary>
        /// The container definition for the events store. This is used to look up the corresponding configuration from the
        /// tenant.
        /// </summary>
        public static readonly CosmosContainerDefinition EventsContainerDefinition
            = new CosmosContainerDefinition("workflow", "instance-events", "/bucketId");

        /// <summary>
        /// The container definition for the snapshots store. This is used to look up the corresponding configuration from the
        /// tenant.
        /// </summary>
        public static readonly CosmosContainerDefinition SnapshotsContainerDefinition
            = new CosmosContainerDefinition("workflow", "instance-snapshots", "/bucketId");

        private readonly ITenantCosmosContainerFactory containerFactory;
        private readonly ILogger<NEventStoreWorkflowInstanceStore> storeLogger;
        private readonly ConcurrentDictionary<string, Task<IStoreEvents>> eventStores = new ConcurrentDictionary<string, Task<IStoreEvents>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantedCosmosDbNEventStoreWorkflowInstanceStoreFactory"/> class.
        /// </summary>
        /// <param name="containerFactory">The <see cref="ITenantCosmosContainerFactory"/> that will be used to create
        /// underlying <see cref="Container"/> instances for the content stores.</param>
        /// <param name="storeLogger">The logger for the workflow instance store.</param>
        public TenantedCosmosDbNEventStoreWorkflowInstanceStoreFactory(
            ITenantCosmosContainerFactory containerFactory,
            ILogger<NEventStoreWorkflowInstanceStore> storeLogger)
        {
            this.containerFactory = containerFactory
                ?? throw new ArgumentNullException(nameof(containerFactory));

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
                _ => this.BuildStore(tenant)).ConfigureAwait(false);

            return new NEventStoreWorkflowInstanceStore(store, this.storeLogger);
        }

        private Task<IStoreEvents> BuildStore(ITenant tenant)
        {
            var settings = new CosmosDbPersistenceSettings(
                () => this.containerFactory.GetContainerForTenantAsync(tenant, EventsContainerDefinition),
                () => this.containerFactory.GetContainerForTenantAsync(tenant, SnapshotsContainerDefinition));

            IStoreEvents result = Wireup.Init()
                .UsingCosmosDbPersistence(settings)
                .UsingJsonSerialization()
                .Build();

            return Task.FromResult(result);
        }
    }
}
