namespace NEventStore.Persistence.CosmosDb.Internal
{
    using System;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Corvus.Retry;
    using Microsoft.Azure.Cosmos;

    public class CosmosDocumentBackedCheckpointGenerator : ICheckpointGenerator
    {
        private static readonly PartitionKey CheckpointDocumentPartitionKey = new PartitionKey(NextCheckpointIdDocument.DefaultId);
        private readonly Func<Task<Container>> containerFactory;

        private NextCheckpointIdDocument? nextCheckpointIdDocument;
        private string? nextCheckpointIdDocumentEtag;

        public CosmosDocumentBackedCheckpointGenerator(Func<Task<Container>> containerFactory)
        {
            this.containerFactory = containerFactory
                ?? throw new ArgumentNullException(nameof(containerFactory));
        }

        public async Task<long> NextAsync(CancellationToken? cancellationToken = null)
        {
            Exception? exception = null;

            // If they haven't given us a token, we'll give up after 5 seconds.
            // TODO: confirm this is a sensible default time.
            cancellationToken ??= new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token;

            while (!cancellationToken.Value.IsCancellationRequested)
            {
                if (this.nextCheckpointIdDocument is null)
                {
                    (this.nextCheckpointIdDocument, this.nextCheckpointIdDocumentEtag) = await this.LoadNextCheckpointDocument().ConfigureAwait(false);
                }

                long result = this.nextCheckpointIdDocument.NextCheckpointId;
                this.nextCheckpointIdDocument.NextCheckpointId++;

                try
                {
                    await this.UpdateNextCheckpointIdDocument(this.nextCheckpointIdDocument, this.nextCheckpointIdDocumentEtag).ConfigureAwait(false);
                    return result;
                }
                catch (CosmosException cex) when (cex.StatusCode == HttpStatusCode.Conflict)
                {
                    // The document has been updated, so our Id is incorrect.
                    // Null out our local copy, then allow the loop to continue. The document will get reloaded and
                    // we can try again.
                    exception = cex;
                    this.nextCheckpointIdDocument = null;
                }
            }

            // If we're here, we've failed.
            throw new StorageException("Failed to acquire a new checkpoint Id.", exception);
        }

        private async Task<(NextCheckpointIdDocument Document, string? ETag)> LoadNextCheckpointDocument()
        {
            Container container = await this.containerFactory().ConfigureAwait(false);

            try
            {
                ItemResponse<NextCheckpointIdDocument> response = await container.ReadItemAsync<NextCheckpointIdDocument>(
                    NextCheckpointIdDocument.DefaultId,
                    CheckpointDocumentPartitionKey).ConfigureAwait(false);

                return (response.Resource, response.ETag);
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return (new NextCheckpointIdDocument(), null);
            }
        }

        private async Task UpdateNextCheckpointIdDocument(NextCheckpointIdDocument document, string? etag)
        {
            Container container = await this.containerFactory().ConfigureAwait(false);

            var options = new ItemRequestOptions();

            if (!string.IsNullOrEmpty(etag))
            {
                options.IfMatchEtag = etag;
            }

            ItemResponse<NextCheckpointIdDocument> result = await container.UpsertItemAsync(
                document,
                CheckpointDocumentPartitionKey, options).ConfigureAwait(false);


        }
    }
}
