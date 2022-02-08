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
        [BeforeFeature("@setupTenantedSqlDatabase", Order = ContainerBeforeFeatureOrder.ServiceProviderAvailable)]
        public static void SetupDatabases(FeatureContext featureContext)
        {
            IServiceProvider serviceProvider = ContainerBindings.GetServiceProvider(featureContext);
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

            LegacyV2CosmosContainerConfiguration cosmosConfig =
                configuration.GetSection("TestCosmosConfiguration").Get<LegacyV2CosmosContainerConfiguration>()
                ?? new LegacyV2CosmosContainerConfiguration();

            cosmosConfig.DatabaseName = "endjinspecssharedthroughput";
            cosmosConfig.DisableTenantIdPrefix = true;

            // Configuring with V2-style configuration because for now, the application uses the
            // ICosmosContainerSourceWithTenantLegacyTransition in the mode where all the tenant
            // configuration remains in V2 mode.
            tenantProvider.Root.UpdateProperties(data => data.Append(new KeyValuePair<string, object>(
                $"StorageConfiguration__{TenantedCosmosWorkflowStoreServiceCollectionExtensions.WorkflowStoreLogicalDatabaseName}__{TenantedCosmosWorkflowStoreServiceCollectionExtensions.WorkflowDefinitionStoreLogicalContainerName}",
                cosmosConfig)));

            tenantProvider.Root.UpdateProperties(data => data.Append(new KeyValuePair<string, object>(
                $"StorageConfiguration__{TenantedCosmosWorkflowStoreServiceCollectionExtensions.WorkflowStoreLogicalDatabaseName}__{TenantedCosmosWorkflowStoreServiceCollectionExtensions.WorkflowInstanceStoreLogicalContainerName}",
                cosmosConfig)));

            tenantProvider.Root.UpdateProperties(data => data.Append(new KeyValuePair<string, object>(
                "StorageConfiguration__workflow__testdocuments",
                cosmosConfig)));

            // This is required by the various bits of the test that work with the "data catalog".
            Container testDocumentsRepository = WorkflowRetryHelper.ExecuteWithStandardTestRetryRulesAsync(
                async () => await factory.GetContainerForTenantAsync(
                    tenantProvider.Root,
                    "StorageConfiguration__workflow__testdocuments",
                    "NotUsed",
                    "workflow",
                    "testdocuments",
                    "/id",
                    cosmosClientOptions: optionsFactory.CreateCosmosClientOptions())).Result;

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
        }
    }
}