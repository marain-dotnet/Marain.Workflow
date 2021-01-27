namespace NEventStore.Persistence.CosmosDb
{
    using System;
    using System.Threading.Tasks;
    using System.Transactions;
    using Microsoft.Azure.Cosmos;
    using NEventStore.Logging;
    using NEventStore.Serialization;

    public class CosmosDbPersistenceWireup : PersistenceWireup
    {
        private static readonly ILog Logger = LogFactory.BuildLogger(typeof(CosmosDbPersistenceWireup));

        public CosmosDbPersistenceWireup(Wireup inner, CosmosDbPersistenceSettings settings)
            : base(inner)
        {
            Logger.Debug("Configuring CosmosDb persistence engine.");

            TransactionScopeOption options = this.Container.Resolve<TransactionScopeOption>();

            if (options != TransactionScopeOption.Suppress)
            {
                Logger.Warn("CosmosDb does not participate in transactions using TransactionScope.");
            }

            this.Container.Register(c => new CosmosDbPersistenceFactory(c.Resolve<ISerialize>(), settings).Build());
        }
    }
}
