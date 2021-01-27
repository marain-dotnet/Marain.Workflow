namespace NEventStore.Persistence.CosmosDb
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos;
    using NEventStore.Logging;
    using NEventStore.Persistence.CosmosDb.Internal;
    using NEventStore.Serialization;

    public class CosmosDbPersistenceEngine : IPersistStreams
    {
        private static readonly ILog Logger = LogFactory.BuildLogger(typeof(CosmosDbPersistenceEngine));

        private readonly CosmosDbPersistenceSettings settings;
        private readonly ICheckpointGenerator checkpointGenerator;
        private readonly ISerialize serializer;

        public CosmosDbPersistenceEngine(CosmosDbPersistenceSettings settings, ISerialize serializer)
        {
            this.settings = settings
                ?? throw new ArgumentNullException(nameof(settings));

            this.serializer = serializer
                ?? throw new ArgumentNullException(nameof(serializer));

            this.checkpointGenerator = new CosmosDocumentBackedCheckpointGenerator(this.settings.EventsContainerFactory);
        }

        public bool IsDisposed { get; private set; }

        public bool AddSnapshot(ISnapshot snapshot)
        {
            throw new NotImplementedException();
        }

        public ICommit Commit(CommitAttempt attempt)
        {
            return AsyncHelper.RunSync(() => this.CommitAsync(attempt));
        }

        public void DeleteStream(string bucketId, string streamId)
        {
            AsyncHelper.RunSync(() => this.DeleteStreamAsync(bucketId, streamId));
        }

        public void Dispose()
        {
            // TODO: Probably don't need this.
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Drop()
        {
            AsyncHelper.RunSync(this.DropAsync);
        }

        public IEnumerable<ICommit> GetFrom(string bucketId, DateTime start)
        {
            return this.GetFromTo(bucketId, start, DateTime.MaxValue);
        }

        public IEnumerable<ICommit> GetFrom(long checkpointToken)
        {
            return AsyncHelper.RunSync(() => this.GetFromAsync(checkpointToken));
        }

        public IEnumerable<ICommit> GetFrom(string bucketId, long checkpointToken)
        {
            return AsyncHelper.RunSync(() => this.GetFromAsync(bucketId, checkpointToken));
        }

        public IEnumerable<ICommit> GetFrom(string bucketId, string streamId, int minRevision, int maxRevision)
        {
            return AsyncHelper.RunSync(() => this.GetFromAsync(bucketId, streamId, minRevision, maxRevision));
        }

        public IEnumerable<ICommit> GetFromTo(string bucketId, DateTime start, DateTime end)
        {
            return AsyncHelper.RunSync(() => this.GetFromToAsync(bucketId, start, end));
        }

        public IEnumerable<ICommit> GetFromTo(long from, long to)
        {
            return AsyncHelper.RunSync(() => this.GetFromToAsync(from, to));
        }

        public IEnumerable<ICommit> GetFromTo(string bucketId, long from, long to)
        {
            return AsyncHelper.RunSync(() => this.GetFromToAsync(bucketId, from, to));
        }

        public ISnapshot GetSnapshot(string bucketId, string streamId, int maxRevision)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IStreamHead> GetStreamsToSnapshot(string bucketId, int maxThreshold)
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            AsyncHelper.RunSync(this.InitializeAsync);
        }

        public void Purge()
        {
            throw new NotImplementedException();
        }

        public void Purge(string bucketId)
        {
            throw new NotImplementedException();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || this.IsDisposed)
            {
                return;
            }

            Logger.Debug("Shutting down persistence.");
            this.IsDisposed = true;
        }

        private async Task InitializeAsync()
        {
            Container container = await this.settings.EventsContainerFactory().ConfigureAwait(false);

            var checkpointKey = new UniqueKey();
            checkpointKey.Paths.Add("/checkpointNumber");

            var uniqueKeyPolicy = new UniqueKeyPolicy();
            uniqueKeyPolicy.UniqueKeys.Add(checkpointKey);

            var props = new ContainerProperties
            {
                UniqueKeyPolicy = uniqueKeyPolicy
            };

            await container.ReplaceContainerAsync(props).ConfigureAwait(false);
        }

        private async Task<ICommit> CommitAsync(CommitAttempt attempt)
        {
            if (Logger.IsDebugEnabled)
            {
                Logger.Debug(
                    "Attempting to commit {0} events on stream '{1}' at sequence {2}.",
                    attempt.Events.Count,
                    attempt.StreamId,
                    attempt.CommitSequence);
            }

            long checkpoint = await this.checkpointGenerator.NextAsync().ConfigureAwait(false);

            var commit = attempt.ToCosmosDbCommit(checkpoint, this.serializer);

            Container container = await this.settings.EventsContainerFactory().ConfigureAwait(false);

            try
            {
                ItemResponse<CosmosDbCommit> result = await container.CreateItemAsync(
                    commit,
                    new PartitionKey(commit.BucketId)).ConfigureAwait(false);

                return result.Resource.ToCommit(this.serializer);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.Conflict)
            {
                throw new ConcurrencyException();
            }
        }

        private Task<IEnumerable<ICommit>> GetFromAsync(long checkpointToken)
        {
            QueryDefinition query = new QueryDefinition(
                @"SELECT *
                    FROM c
                  WHERE c.contentType = @contentType
                    AND c.checkpointNumber >= @checkpointToken")
                .WithParameter("@contentType", CosmosDbCommit.RegisteredContentType)
                .WithParameter("@checkpointToken", checkpointToken);

            return this.GetCommitsAsync(query);
        }

        private Task<IEnumerable<ICommit>> GetFromAsync(string bucketId, long checkpointToken)
        {
            QueryDefinition query = new QueryDefinition(
                @"SELECT *
                    FROM c
                  WHERE c.bucketId = @bucketId 
                    AND c.contentType = @contentType
                    AND c.checkpointNumber >= @checkpointToken")
                .WithParameter("@bucketId", bucketId)
                .WithParameter("@contentType", CosmosDbCommit.RegisteredContentType)
                .WithParameter("@checkpointToken", checkpointToken);

            return this.GetCommitsAsync(query, bucketId);
        }

        private Task<IEnumerable<ICommit>> GetFromAsync(string bucketId, string streamId, int minRevision, int maxRevision)
        {
            QueryDefinition query = new QueryDefinition(
                @"SELECT *
                    FROM c
                  WHERE c.bucketId = @bucketId 
                    AND c.contentType = @contentType
                    AND c.streamId = @streamId
                    AND c.streamRevisionTo >= @minRevision
                    AND c.streamRevisionFrom <= @maxRevision")
                .WithParameter("@bucketId", bucketId)
                .WithParameter("@contentType", CosmosDbCommit.RegisteredContentType)
                .WithParameter("@streamId", streamId)
                .WithParameter("@minRevision", minRevision)
                .WithParameter("@maxRevision", maxRevision);

            return this.GetCommitsAsync(query, bucketId);
        }

        private Task<IEnumerable<ICommit>> GetFromToAsync(string bucketId, DateTime start, DateTime end)
        {
            QueryDefinition query = new QueryDefinition(
                @"SELECT *
                    FROM c
                  WHERE c.bucketId = @bucketId 
                    AND c.contentType = @contentType
                    AND c.commitStamp >= @start
                    AND c.commitStamp < @end")
                .WithParameter("@bucketId", bucketId)
                .WithParameter("@contentType", CosmosDbCommit.RegisteredContentType)
                .WithParameter("@start", start)
                .WithParameter("@end", end);

            return this.GetCommitsAsync(query, bucketId);
        }

        private Task<IEnumerable<ICommit>> GetFromToAsync(long from, long to)
        {
            QueryDefinition query = new QueryDefinition(
                @"SELECT *
                    FROM c
                  WHERE c.contentType = @contentType
                    AND c.checkpointNumber > @from
                    AND c.checkpointNumber <= @to")
                .WithParameter("@contentType", CosmosDbCommit.RegisteredContentType)
                .WithParameter("@from", from)
                .WithParameter("@to", to);

            return this.GetCommitsAsync(query);
        }

        private Task<IEnumerable<ICommit>> GetFromToAsync(string bucketId, long from, long to)
        {
            QueryDefinition query = new QueryDefinition(
                @"SELECT *
                    FROM c
                  WHERE c.bucketId = @bucketId 
                    AND c.contentType = @contentType
                    AND c.checkpointNumber > @from
                    AND c.checkpointNumber <= @to")
                .WithParameter("@bucketId", bucketId)
                .WithParameter("@contentType", CosmosDbCommit.RegisteredContentType)
                .WithParameter("@from", from)
                .WithParameter("@to", to);

            return this.GetCommitsAsync(query, bucketId);
        }

        public async Task DeleteStreamAsync(string bucketId, string streamId)
        {
            Container container = await this.settings.EventsContainerFactory().ConfigureAwait(false);

            // TODO: Make this more efficient by only returning Ids of items to delete.
            QueryDefinition query = new QueryDefinition(
                @"SELECT c.id
                    FROM c
                  WHERE c.bucketId = @bucketId 
                    AND c.contentType = @contentType
                    AND c.streamId = @streamId")
                .WithParameter("@bucketId", bucketId)
                .WithParameter("@contentType", CosmosDbCommit.RegisteredContentType)
                .WithParameter("@streamId", streamId);

            IEnumerable<string> ids = await this.ExecuteQueryAllAsync<string>(query, bucketId).ConfigureAwait(false);
            var partitionKey = new PartitionKey(bucketId);
            IEnumerable<Task<ItemResponse<CosmosDbCommit>>> tasks = ids.Select(x => container.DeleteItemAsync<CosmosDbCommit>(x, partitionKey));
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        private async Task<IEnumerable<ICommit>> GetCommitsAsync(QueryDefinition queryDefinition, string partitionKey = null)
        {
            IEnumerable<CosmosDbCommit> commits = await this.ExecuteQueryAllAsync<CosmosDbCommit>(queryDefinition, partitionKey).ConfigureAwait(false);
            return commits.Select(x => x.ToCommit(this.serializer));
        }

        private async Task<IEnumerable<T>> ExecuteQueryAllAsync<T>(QueryDefinition queryDefinition, string partitionKey = null)
        {
            var results = new List<T>();

            Container container = await this.settings.EventsContainerFactory().ConfigureAwait(false);
            string continuationToken = null;

            var options = new QueryRequestOptions
            {
                // TODO: Emulator doesn't support consistency levels; need to deal with this somehow.
                //// ConsistencyLevel = ConsistencyLevel.Strong,
                PartitionKey = string.IsNullOrEmpty(partitionKey) ? (PartitionKey?)null : new PartitionKey(partitionKey),
            };

            do
            {
                FeedIterator<T> iterator = container.GetItemQueryIterator<T>(queryDefinition, continuationToken, options);

                if (iterator.HasMoreResults)
                {
                    FeedResponse<T> currentResults = await iterator.ReadNextAsync().ConfigureAwait(false);
                    results.AddRange(currentResults);
                    continuationToken = currentResults.ContinuationToken;
                }
            }
            while (!string.IsNullOrEmpty(continuationToken));

            return results;
        }

        private async Task DropAsync()
        {
            // TODO: Add the snapshot container too.
            Container container = await this.settings.EventsContainerFactory().ConfigureAwait(false);
            await container.DeleteContainerAsync().ConfigureAwait(false);
        }
    }
}
