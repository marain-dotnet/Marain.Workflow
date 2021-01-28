namespace NEventStore.Persistence.CosmosDb.Internal
{
    public class CosmosDbSnapshot
    {
        public const string RegisteredContentType = "application/vnd.neventstore.cosmossnapshot.v1";

        public CosmosDbSnapshot(string bucketId, string streamId, int streamRevision, string payload)
        {
            this.BucketId = bucketId;
            this.StreamId = streamId;
            this.StreamRevision = streamRevision;
            this.Payload = payload;
        }

        public string ContentType => RegisteredContentType;

        public string Id => $"snapshot-{this.BucketId}-{this.StreamId}-{this.StreamRevision}";

        public string BucketId { get; }

        public string StreamId { get; }

        public int StreamRevision { get; }

        public string Payload { get; }
    }
}
