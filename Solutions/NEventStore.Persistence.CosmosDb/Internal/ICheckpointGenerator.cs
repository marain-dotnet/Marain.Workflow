namespace NEventStore.Persistence.CosmosDb.Internal
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface ICheckpointGenerator
    {
        Task<long> NextAsync(CancellationToken? cancellationToken = null);
    }
}
