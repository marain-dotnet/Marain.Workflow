namespace NEventStore.Persistence.CosmosDb
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos;
    using NEventStore.Serialization;

    public class CosmosDbPersistenceFactory : IPersistenceFactory
    {
        private readonly CosmosDbPersistenceSettings settings;
        private readonly ISerialize serializer;

        public CosmosDbPersistenceFactory(ISerialize serializer, CosmosDbPersistenceSettings settings)
        {
            this.settings = settings
                ?? throw new ArgumentNullException(nameof(settings));

            this.serializer = serializer
                ?? throw new ArgumentNullException(nameof(serializer));
        }

        public IPersistStreams Build()
        {
            return new CosmosDbPersistenceEngine(this.settings, this.serializer);
        }
    }
}
