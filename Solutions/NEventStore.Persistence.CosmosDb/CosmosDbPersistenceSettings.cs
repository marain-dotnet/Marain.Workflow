namespace NEventStore.Persistence.CosmosDb
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos;

    public class CosmosDbPersistenceSettings
    {
        public CosmosDbPersistenceSettings(
            Func<Task<Container>> eventsContainerFactory,
            Func<Task<Container>> snapshotContainerFactory)
        {
            this.EventsContainerFactory = eventsContainerFactory;
            this.SnapshotContainerFactory = snapshotContainerFactory;
        }

        public Func<Task<Container>> EventsContainerFactory { get; }

        public Func<Task<Container>> SnapshotContainerFactory { get; }
    }
}
