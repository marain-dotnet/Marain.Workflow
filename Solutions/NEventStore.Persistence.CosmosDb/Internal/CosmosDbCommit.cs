namespace NEventStore.Persistence.CosmosDb.Internal
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class CosmosDbCommit
    {
        public const string RegisteredContentType = "application/vnd.neventstore.commit.v1";

        public string ContentType => RegisteredContentType;

        public string Id => $"{this.BucketId}-{this.StreamId}-{this.CommitSequence}";

        public string BucketId { get; set; }

        public string StreamId { get; set; }

        public int CommitSequence { get; set; }

        public Guid CommitId { get; set; }

        public DateTime CommitStamp { get; set; }

        public IDictionary<string, object> Headers { get; set; }

        public string Payload { get; set; }

        public bool Dispatched { get; set; }

        public long CheckpointNumber { get; set; }

        public int StreamRevisionFrom { get; set; }

        public int StreamRevisionTo { get; set; }
    }
}
