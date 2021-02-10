namespace NEventStore.Persistence.CosmosDb.Internal
{
    using System;
    using System.Collections.Generic;
    using NEventStore.Serialization;

    public static class DataExtensions
    {
        public static CosmosDbCommit ToCosmosDbCommit(this CommitAttempt commit, long checkpoint, ISerialize serializer)
        {
            byte[] serializedPayload = serializer.Serialize(commit.Events);

            return new CosmosDbCommit(
                commit.CommitId,
                commit.CommitStamp,
                commit.BucketId,
                commit.StreamId,
                commit.StreamRevision - commit.Events.Count + 1,
                commit.StreamRevision,
                commit.CommitSequence,
                commit.Headers,
                checkpoint,
                Convert.ToBase64String(serializedPayload),
                false);
        }

        public static CosmosDbSnapshot ToCosmosDbSnapshot(this ISnapshot snapshot, ISerialize serializer)
        {
            byte[] serializedPayload = serializer.Serialize(snapshot.Payload);

            return new CosmosDbSnapshot(
                snapshot.BucketId,
                snapshot.StreamId,
                snapshot.StreamRevision,
                Convert.ToBase64String(serializedPayload));
        }

        public static Commit? ToCommit(this CosmosDbCommit cosmosCommit, ISerialize serializer)
        {
            if (cosmosCommit is null)
            {
                return null;
            }

            ICollection<EventMessage> deserializedPayload = serializer.Deserialize<ICollection<EventMessage>>(Convert.FromBase64String(cosmosCommit.Payload));

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

        public static ISnapshot? ToSnapshot(this CosmosDbSnapshot cosmosSnapshot, ISerialize serializer)
        {
            if (cosmosSnapshot is null)
            {
                return null;
            }

            object deserializedPayload = serializer.Deserialize<object>(Convert.FromBase64String(cosmosSnapshot.Payload));

            return new Snapshot(
                cosmosSnapshot.BucketId,
                cosmosSnapshot.StreamId,
                cosmosSnapshot.StreamRevision,
                deserializedPayload);
        }
    }
}
