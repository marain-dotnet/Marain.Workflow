namespace NEventStore.Persistence.CosmosDb.Internal
{
    using System.Threading.Tasks;

    public interface ICheckpointGenerator
    {
        Task<long> NextAsync();
    }
}
