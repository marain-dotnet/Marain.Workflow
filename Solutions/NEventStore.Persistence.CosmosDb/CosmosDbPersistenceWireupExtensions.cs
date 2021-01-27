namespace NEventStore.Persistence.CosmosDb
{
    public static class CosmosDbPersistenceWireupExtensions
    {
        public static PersistenceWireup UsingCosmosDbPersistence(this Wireup wireup, CosmosDbPersistenceSettings settings)
        {
            return new CosmosDbPersistenceWireup(wireup, settings);
        }
    }
}
