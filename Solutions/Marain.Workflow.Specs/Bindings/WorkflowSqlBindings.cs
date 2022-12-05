// <copyright file="WorkflowSqlBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Specs.Bindings
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Corvus.CosmosClient;
    using Corvus.Storage.Azure.Cosmos;
    using Corvus.Storage.Azure.Cosmos.Tenancy;
    using Corvus.Storage.Sql;
    using Corvus.Storage.Sql.Tenancy;
    using Corvus.Tenancy;
    using Corvus.Testing.SpecFlow;

    using Marain.Workflows.Specs.Steps;

    using Microsoft.Azure.Cosmos;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using TechTalk.SpecFlow;

    /// <summary>
    /// Specflow bindings to support Cosmos DB.
    /// </summary>
    [Binding]
    public static class WorkflowSqlBindings
    {
        /// <summary>
        /// The key for the document repository instance in the feature context.
        /// </summary>
        public const string TestDocumentsRepository = "TestDocumentsRepository";

        /// <summary>
        /// Set up a SQL Database for the feature.
        /// </summary>
        /// <param name="featureContext">The feature context.</param>
        /// <returns>A <see cref="Task"/> indicating when setup completes.</returns>
        [BeforeFeature("@setupTenantedSqlDatabase", Order = ContainerBeforeFeatureOrder.ServiceProviderAvailable)]
        public static async Task SetupDatabases(FeatureContext featureContext)
        {
            // We need each test run to have a distinct container. We want these test-generated
            // containers to be easily recognized in storage accounts, so we don't just want to use
            // GUIDs.
            string testRunId = DateTime.Now.ToString("yyyy-MM-dd-hhmmssfff");

            IServiceProvider serviceProvider = ContainerBindings.GetServiceProvider(featureContext);
            ICosmosContainerSourceFromDynamicConfiguration cosmosContainerSource = serviceProvider.GetRequiredService<ICosmosContainerSourceFromDynamicConfiguration>();
            ISqlConnectionFromDynamicConfiguration sqlConnectionFactory = serviceProvider.GetRequiredService<ISqlConnectionFromDynamicConfiguration>();
            ICosmosContainerSourceWithTenantLegacyTransition factory = serviceProvider.GetRequiredService<ICosmosContainerSourceWithTenantLegacyTransition>();
            ICosmosOptionsFactory optionsFactory = serviceProvider.GetRequiredService<ICosmosOptionsFactory>();
            ITenantProvider tenantProvider = serviceProvider.GetRequiredService<ITenantProvider>();
            IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();

            string containerBase = Guid.NewGuid().ToString();
            string databaseName = $"workflow-{containerBase}";

            SqlDatabaseConfiguration sqlConfig = new()
            {
                ConnectionStringPlainText = $"Server=(localdb)\\mssqllocaldb;Initial Catalog={databaseName};Trusted_Connection=True;MultipleActiveResultSets=true",
            };
            tenantProvider.Root.UpdateProperties(data => data.AddSqlDatabaseConfiguration(
                TenantedSqlWorkflowStoreServiceCollectionExtensions.WorkflowConnectionKey,
                sqlConfig));

            CosmosContainerConfiguration cosmosConfig =
                configuration.GetSection("TestCosmosConfiguration").Get<CosmosContainerConfiguration>()
                    ?? new CosmosContainerConfiguration();

            cosmosConfig.Database = "endjinspecssharedthroughput";
            CosmosContainerConfiguration appDataStoreConfig = cosmosConfig with
            {
                Container = $"specs-workflow-testdocuments-{testRunId}",
            };

            tenantProvider.Root.UpdateProperties(data => data.Concat(new Dictionary<string, object>
            {
                { "StorageConfiguration__workflow__testdocuments", appDataStoreConfig },
            }));

            // This is required by the various bits of the test that work with the "data catalog".
            Container testDocumentsRepository = await WorkflowRetryHelper.ExecuteWithStandardTestRetryRulesAsync(
                async () => await factory.GetContainerForTenantAsync(
                    tenantProvider.Root,
                    "NotUsed",
                    "StorageConfiguration__workflow__testdocuments",
                    "workflow",
                    "testdocuments",
                    "/id",
                    cosmosClientOptions: optionsFactory.CreateCosmosClientOptions())).ConfigureAwait(false);
            await testDocumentsRepository.Database
                .CreateContainerIfNotExistsAsync(
                    testDocumentsRepository.Id,
                    "/id")
                .ConfigureAwait(false);

            featureContext.Set(testDocumentsRepository, TestDocumentsRepository);

            // And now, deploy the sql server for this instance.
#if DEBUG
            const string BUILD = "debug";
#else
            const string BUILD = "release";
#endif

            SqlHelpers.SetupDatabaseFromDacPac(sqlConfig.ConnectionStringPlainText, databaseName, @$"..\..\..\..\Marain.Workflow.Storage.Sql.Database\bin\{BUILD}\Marain.Workflow.Storage.Sql.Database.dacpac");
        }

        /// <summary>
        /// Tear down the cosmos DB repository for the feature.
        /// </summary>
        /// <param name="featureContext">The feature context.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [AfterFeature("@setupTenantedSqlDatabase", Order = 100000)]
        public static async Task TeardownDatabases(FeatureContext featureContext)
        {
            IServiceProvider serviceProvider = ContainerBindings.GetServiceProvider(featureContext);
            ISqlConnectionFromDynamicConfiguration sqlConnectionFactory = serviceProvider.GetRequiredService<ISqlConnectionFromDynamicConfiguration>();
            ITenantProvider tenantProvider = serviceProvider.GetRequiredService<ITenantProvider>();

            SqlDatabaseConfiguration config = tenantProvider.Root.GetSqlDatabaseConfiguration(TenantedSqlWorkflowStoreServiceCollectionExtensions.WorkflowConnectionKey);

            await featureContext.RunAndStoreExceptionsAsync(async () =>
            {
                SqlConnection connection = await sqlConnectionFactory.GetStorageContextAsync(config).ConfigureAwait(false);
                SqlHelpers.DeleteDatabase(connection, connection.Database);
            });

            await featureContext.RunAndStoreExceptionsAsync(
                () => featureContext.Get<Container>(TestDocumentsRepository).DeleteContainerAsync()).ConfigureAwait(false);
        }
    }
}