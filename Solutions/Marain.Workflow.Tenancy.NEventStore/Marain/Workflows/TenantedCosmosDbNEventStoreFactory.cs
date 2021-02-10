// <copyright file="TenantedCosmosDbNEventStoreFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System;
    using System.Threading.Tasks;
    using Corvus.Azure.Cosmos.Tenancy;
    using Corvus.Tenancy;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Extensions.Logging;
    using NEventStore;
    using NEventStore.Persistence.CosmosDb;
    using NEventStore.Serialization.Json;

    /// <summary>
    /// Retrieves an <see cref="IStoreEvents"/> instance backed by tenanted Cosmos storage.
    /// </summary>
    public class TenantedCosmosDbNEventStoreFactory : ITenantedNEventStoreFactory
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

        /// <summary>
        /// Initializes a new instance of the <see cref="TenantedCosmosDbNEventStoreFactory"/> class.
        /// </summary>
        /// <param name="containerFactory">The <see cref="ITenantCosmosContainerFactory"/> that will be used to create
        /// underlying <see cref="Container"/> instances for the content stores.</param>
        public TenantedCosmosDbNEventStoreFactory(
            ITenantCosmosContainerFactory containerFactory)
        {
            this.containerFactory = containerFactory
                ?? throw new ArgumentNullException(nameof(containerFactory));
        }

        /// <inheritdoc/>
        public Task<IStoreEvents> GetStoreForTenantAsync(ITenant tenant)
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
