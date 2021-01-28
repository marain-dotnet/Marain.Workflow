namespace NEventStore.Persistence.CosmosDb.Internal
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class CosmosDbCommit
    {
        public const string RegisteredContentType = "application/vnd.neventstore.cosmoscommit.v1";

        public CosmosDbCommit(
            Guid commitId,
            DateTime commitStamp,
            string bucketId,
            string streamId,
            int streamRevisionFrom,
            int streamRevisionTo,
            int commitSequence,
            IDictionary<string, object> headers,
            long checkpointNumber,
            string payload,
            bool dispatched)
        {
            this.BucketId = bucketId;
            this.StreamId = streamId;
            this.CommitSequence = commitSequence;
            this.CommitId = commitId;
            this.CommitStamp = commitStamp;
            this.Headers = headers;
            this.Payload = payload;
            this.Dispatched = dispatched;
            this.CheckpointNumber = checkpointNumber;
            this.StreamRevisionFrom = streamRevisionFrom;
            this.StreamRevisionTo = streamRevisionTo;
        }

        public string ContentType => RegisteredContentType;

        public string Id => $"{this.BucketId}-{this.StreamId}-{this.CommitSequence}";

        public string BucketId { get; }

        public string StreamId { get; }

        public int CommitSequence { get; }

        public Guid CommitId { get; }

        public DateTime CommitStamp { get; }

        public IDictionary<string, object> Headers { get; }

        public string Payload { get; }

        public bool Dispatched { get; }

        public long CheckpointNumber { get; }

        public int StreamRevisionFrom { get; }

        public int StreamRevisionTo { get; }
    }
}
