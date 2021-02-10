// <copyright file="TenantedInMemoryNEventStoreFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System.Threading.Tasks;
    using Corvus.Tenancy;
    using NEventStore;
    using NEventStore.Serialization.Json;

    /// <summary>
    /// Retrieves an <see cref="IStoreEvents"/> instance backed by tenanted Cosmos storage.
    /// </summary>
    public class TenantedInMemoryNEventStoreFactory : ITenantedNEventStoreFactory
    {
        /// <inheritdoc/>
        public Task<IStoreEvents> GetStoreForTenantAsync(ITenant tenant)
        {
            IStoreEvents result = Wireup.Init()
                .UsingInMemoryPersistence()
                .UsingJsonSerialization()
                .Build();

            return Task.FromResult(result);
        }
    }
}
