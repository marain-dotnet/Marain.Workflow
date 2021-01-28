namespace NEventStore.Persistence.CosmosDb.Internal
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Corvus.Retry;
    using Microsoft.Azure.Cosmos;

    public class CosmosDocumentBackedCheckpointGenerator : ICheckpointGenerator
    {
        private readonly Func<Task<Container>> containerFactory;

        public CosmosDocumentBackedCheckpointGenerator(Func<Task<Container>> containerFactory)
        {
            this.containerFactory = containerFactory
                ?? throw new ArgumentNullException(nameof(containerFactory));
        }

        public async Task<long> NextAsync()
        {
            Container container = await this.containerFactory().ConfigureAwait(false);
            var partitionKey = new PartitionKey(NextCheckpointIdDocument.DefaultId);

            // TODO: Everything from here on down has to be in a retry block. Rather than taking a dependency on
            // Corvus.Retry we should manually implement here.
            // TODO: Rather than query every time, we should stash the most recent value and only re-query if we get
            // a conflict.
            return await Retriable.RetryAsync(
                async () =>
                {
                    // Query for the current checkpoint value.
                    string? etag = null;
                    NextCheckpointIdDocument nextIdDocument;

                    try
                    {
                        ItemResponse<NextCheckpointIdDocument> response = await container.ReadItemAsync<NextCheckpointIdDocument>(
                            NextCheckpointIdDocument.DefaultId,
                            partitionKey).ConfigureAwait(false);

                        nextIdDocument = response.Resource;
                        etag = response.ETag;
                    }
                    catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
                    {
                        nextIdDocument = new NextCheckpointIdDocument();
                    }

                    long nextId = nextIdDocument.NextCheckpointId;

                    ++nextIdDocument.NextCheckpointId;
                    var options = new ItemRequestOptions();

                    if (!string.IsNullOrEmpty(etag))
                    {
                        options.IfMatchEtag = etag;
                    }

                    await container.UpsertItemAsync(nextIdDocument, partitionKey, options).ConfigureAwait(false);

                    return nextId;
                }).ConfigureAwait(false);
        }
    }
}
