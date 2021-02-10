// <copyright file="ITenantedNEventStoreFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System.Threading.Tasks;
    using Corvus.Tenancy;
    using NEventStore;

    /// <summary>
    /// Interface for factory classes which build tenanted instances of <see cref="IStoreEvents"/>.
    /// </summary>
    public interface ITenantedNEventStoreFactory
    {
        /// <summary>
        /// Retrieves a store for the specified tenant.
        /// </summary>
        /// <param name="tenant">The tenant to retrieve a store for.</param>
        /// <returns>The store.</returns>
        Task<IStoreEvents> GetStoreForTenantAsync(ITenant tenant);
    }
}
