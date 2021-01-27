namespace NEventStore.Persistence.CosmosDb.Internal
{
    public class NextCheckpointIdDocument
    {
        public const string DefaultId = "CurrentCheckpoint";

        public string Id { get; } = DefaultId;

        public string BucketId { get; } = DefaultId;

        public long NextCheckpointId { get; set; } = 1;
    }
}
