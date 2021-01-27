namespace NEventStore.Persistence.CosmosDb.Internal
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using NEventStore.Serialization;

    public static class DataExtensions
    {
        public static CosmosDbCommit ToCosmosDbCommit(this CommitAttempt commit, long checkpoint, ISerialize serializer)
        {
            byte[] serializedPayload = serializer.Serialize(commit.Events);

            return new CosmosDbCommit
            {
                CheckpointNumber = checkpoint,
                CommitId = commit.CommitId,
                CommitStamp = commit.CommitStamp,
                Headers = commit.Headers,
                Payload = Encoding.UTF8.GetString(serializedPayload),
                StreamRevisionFrom = commit.StreamRevision - commit.Events.Count + 1,
                StreamRevisionTo = commit.StreamRevision,
                BucketId = commit.BucketId,
                StreamId = commit.StreamId,
                CommitSequence = commit.CommitSequence
            };
        }

        public static Commit ToCommit(this CosmosDbCommit cosmosCommit, ISerialize serializer)
        {
            if (cosmosCommit is null)
            {
                return null;
            }

            ICollection<EventMessage> deserializedPayload = serializer.Deserialize<ICollection<EventMessage>>(Encoding.UTF8.GetBytes(cosmosCommit.Payload));

            return new Commit(
                cosmosCommit.BucketId,
                cosmosCommit.StreamId,
                cosmosCommit.StreamRevisionTo,
                cosmosCommit.CommitId,
                cosmosCommit.CommitSequence,
                cosmosCommit.CommitStamp,
                cosmosCommit.CheckpointNumber,
                cosmosCommit.Headers,
                deserializedPayload);
        }
    }
}
