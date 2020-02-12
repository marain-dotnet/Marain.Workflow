// <copyright file="WorkflowCosmosDbBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Specs.Bindings
{
    using System;
    using System.Threading.Tasks;
    using Corvus.Azure.Cosmos.Tenancy;
    using Corvus.SpecFlow.Extensions;
    using Corvus.Tenancy;
    using Marain.Workflows.Specs.Steps;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Extensions.DependencyInjection;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Specflow bindings to support Cosmos DB.
    /// </summary>
    [Binding]
    public static class WorkflowCosmosDbBindings
    {
        /// <summary>
        /// The key for the document repository instance in the feature context.
        /// </summary>
        public const string TestDocumentsRepository = "TestDocumentsRepository";

        /// <summary>
        /// Set up a Cosmos DB Repository for the feature.
        /// </summary>
        /// <param name="featureContext">The feature context.</param>
        /// <remarks>
        /// Note that this sets up a resource in Azure and will incur cost. Ensure the corresponding tear down operation
        /// is always run, or verify manually after a test run.
        /// </remarks>
        [BeforeFeature("@setupTenantedCosmosContainers", Order = ContainerBeforeFeatureOrder.ServiceProviderAvailable)]
        public static void SetupCosmosDbRepository(FeatureContext featureContext)
        {
            IServiceProvider serviceProvider = ContainerBindings.GetServiceProvider(featureContext);
            ITenantCosmosContainerFactory factory = serviceProvider.GetRequiredService<ITenantCosmosContainerFactory>();
            ITenantProvider tenantProvider = serviceProvider.GetRequiredService<ITenantProvider>();

            ITenant rootTenant = tenantProvider.Root;

            CosmosConfiguration config = tenantProvider.Root.GetDefaultCosmosConfiguration();
            config.DatabaseName = "endjinspecssharedthroughput";
            config.DisableTenantIdPrefix = true;
            tenantProvider.Root.SetDefaultCosmosConfiguration(config);

            string containerBase = Guid.NewGuid().ToString();

            Container testDocumentsRepository = WorkflowRetryHelper.ExecuteWithStandardTestRetryRulesAsync(
                () => factory.GetContainerForTenantAsync(
                    rootTenant,
                    new CosmosContainerDefinition("workflow", $"{containerBase}testdocuments", "/id"))).Result;
            featureContext.Set(testDocumentsRepository, TestDocumentsRepository);
        }

        /// <summary>
        /// Tear down the cosmos DB repository for the feature.
        /// </summary>
        /// <param name="featureContext">The feature context.</param>
        /// <returns>A <see cref="Task" /> which completes once the operation has completed.</returns>
        [AfterFeature("@setupTenantedCosmosContainers", Order = 100000)]
        public static async Task TeardownCosmosDb(FeatureContext featureContext)
        {
            // TODO: How do we delete the workflow containers?
            await DeleteRepository(featureContext, TestDocumentsRepository).ConfigureAwait(false);
        }

        /// <summary>
        /// Helper method to delete a document collection from CosmosDb.
        /// </summary>
        /// <param name="featureContext">
        /// The feature context.
        /// </param>
        /// <param name="contextKey">
        /// The key used to retrieve the repository from the feature context.
        /// </param>
        /// <returns>
        /// A <see cref="Task" /> that completes when the repository has been deleted.
        /// </returns>
        private static Task DeleteRepository(FeatureContext featureContext, string contextKey)
        {
            return featureContext.RunAndStoreExceptionsAsync(
                async () => await featureContext.Get<Container>(contextKey).DeleteContainerAsync().ConfigureAwait(false));
        }
    }
}