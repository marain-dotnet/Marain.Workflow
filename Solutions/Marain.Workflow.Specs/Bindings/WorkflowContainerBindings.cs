// <copyright file="WorkflowContainerBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Specs.Bindings
{
    using System.Linq;
    using Corvus.Azure.Cosmos.Tenancy;
    using Corvus.Identity.ManagedServiceIdentity.ClientAuthentication;
    using Corvus.SpecFlow.Extensions;
    using Corvus.Sql.Tenancy;
    using Corvus.Tenancy;
    using Marain.Workflows.Specs.TestObjects;
    using Marain.Workflows.Specs.TestObjects.Subjects;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Container related bindings to configure the service provider for features.
    /// </summary>
    [Binding]
    public static class WorkflowContainerBindings
    {
        /// <summary>
        /// Initializes the container before each feature's tests are run.
        /// </summary>
        /// <param name="featureContext">
        /// The feature context.
        /// </param>
        [BeforeFeature("@perFeatureContainer", Order = ContainerBeforeFeatureOrder.PopulateServiceCollection)]
        public static void InitializeContainer(FeatureContext featureContext)
        {
            ContainerBindings.ConfigureServices(
                featureContext,
                services =>
                {
                    IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
                        .AddEnvironmentVariables()
                        .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);

                    IConfiguration root = configurationBuilder.Build();

                    services.AddSingleton(root);

                    services.AddLogging();

                    services.AddJsonSerializerSettings();

                    services.AddSingleton<ITenantProvider, FakeTenantProvider>();

                    TenantCosmosContainerFactoryOptions cosmosConfiguration = root.GetSection("TenantCosmosContainerFactoryOptions").Get<TenantCosmosContainerFactoryOptions>()
                        ?? new TenantCosmosContainerFactoryOptions();
                    if (cosmosConfiguration.RootTenantCosmosConfiguration == null)
                    {
                        cosmosConfiguration.RootTenantCosmosConfiguration = new CosmosConfiguration();
                    }

                    services.AddTenantCosmosContainerFactory(cosmosConfiguration);

                    TenantSqlConnectionFactoryOptions sqlConfiguration = root.GetSection("TenantSqlConnectionFactoryOptions").Get<TenantSqlConnectionFactoryOptions>()
                        ?? new TenantSqlConnectionFactoryOptions();
                    if (sqlConfiguration.RootTenantSqlConfiguration == null)
                    {
                        sqlConfiguration.RootTenantSqlConfiguration = new SqlConfiguration();
                    }

                    services.AddTenantSqlConnectionFactory(sqlConfiguration);

                    services.AddInMemoryWorkflowTriggerQueue();
                    services.AddInMemoryLeasing();

                    services.RegisterCoreWorkflowContentTypes();
                    services.AddTenantedWorkflowEngineFactory();

                    if (featureContext.FeatureInfo.Tags.Any(t => t == "useCosmosStores"))
                    {
                        services.AddTenantedAzureCosmosWorkflowStore();
                        services.AddTenantedAzureCosmosWorkflowInstanceStore();
                    }
                    else if (featureContext.FeatureInfo.Tags.Any(t => t == "useSqlStores"))
                    {
                        services.AddTenantedSqlWorkflowStore();
                        services.AddTenantedSqlWorkflowInstanceStore();
                    }

                    services.AddContent(factory => factory.RegisterTestContentTypes());

                    services.AddSingleton<DataCatalogItemRepositoryFactory>();

                    services.AddSingleton<IServiceIdentityTokenSource, FakeServiceIdentityTokenSource>();
                });
        }
    }
}