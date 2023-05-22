// <copyright file="TestStorageExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

#pragma warning disable

namespace Microsoft.Extensions.DependencyInjection
{
    using System.Threading.Tasks;
    using Corvus.Storage;
    using Marain.Workflows;
    using Marain.Workflows.Specs.Steps;
    using Marain.Workflows.Specs.TestObjects;

    public static class TestStorageExtensions
    {
        public static IServiceCollection AddInMemoryWorkflowTriggerQueue(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IWorkflowMessageQueue, InMemoryWorkflowMessageQueue>();

            return serviceCollection;
        }

        public static async Task InsertOrOverwriteWorkflowAsync(this IWorkflowStore store, string workflowId, Workflow workflow)
        {
            await WorkflowRetryHelper.ExecuteWithStandardTestRetryRulesAsync(async () =>
            {
                string eTag = null;
                try
                {
                    EntityWithETag<Workflow> oldWorkflowWithETag = await store.GetWorkflowAsync(workflowId).ConfigureAwait(false);
                    eTag = oldWorkflowWithETag.ETag;
                }
                catch (WorkflowNotFoundException)
                {   
                    // Don't care if there is no old workflow.
                }

                await store.UpsertWorkflowAsync(workflow, eTagExpected: eTag).ConfigureAwait(false);
            }).ConfigureAwait(false);
        }
    }
}

#pragma warning restore