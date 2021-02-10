// <copyright file="WorkflowContainerBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Specs.Bindings
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Corvus.Azure.Cosmos.Tenancy;
    using Corvus.Identity.ManagedServiceIdentity.ClientAuthentication;
    using Corvus.Json;
    using Corvus.Tenancy;
    using Corvus.Testing.SpecFlow;
    using Marain.Workflows.Specs.TestObjects;
    using Marain.Workflows.Specs.TestObjects.Subjects;
    using Microsoft.Azure.Cosmos;
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
        /// Initialises the content type factory for use with specs that are more "unit test" in nature.
        /// </summary>
        /// <param name="scenarioContext">The current scenario context.</param>
        [BeforeScenario("@perScenarioContainer", Order = ContainerBeforeScenarioOrder.PopulateServiceCollection)]
        public static void InitializeContainerForScenario(ScenarioContext scenarioContext)
        {
            ContainerBindings.ConfigureServices(
                scenarioContext,
                services =>
                {
                    IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
                        .AddEnvironmentVariables()
                        .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);

                    IConfiguration root = configurationBuilder.Build();

                    services.AddSingleton(root);

                    services.AddLogging();
                    services.AddJsonSerializerSettings();
                    services.RegisterCoreWorkflowContentTypes();
                    services.AddContent(factory => factory.RegisterTestContentTypes());
                });
        }

        [BeforeScenario("@perScenarioContainer", Order = ContainerBeforeScenarioOrder.ServiceProviderAvailable)]
        public static void CreateScenarioTenant(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            IServiceProvider sp = ContainerBindings.GetServiceProvider(scenarioContext);

            IPropertyBagFactory propertyBagFactory = sp.GetRequiredService<IPropertyBagFactory>();

            IEnumerable<KeyValuePair<string, object>> newConfig = Enumerable.Empty<KeyValuePair<string, object>>();

            if (scenarioContext.ScenarioInfo.Tags.Contains("usingCosmosDbNEventStore")
                || featureContext.FeatureInfo.Tags.Contains("usingCosmosDbNEventStore"))
            {
                // Add CosmosConfiguration for the store.
                // TODO: Wire this up to config so we can run against real storage during the CI builds
                newConfig = newConfig.AddCosmosConfiguration(
                    TenantedCosmosDbNEventStoreFactory.EventsContainerDefinition,
                    new CosmosConfiguration { DatabaseName = "workflowspecs", DisableTenantIdPrefix = true });

                newConfig = newConfig.AddCosmosConfiguration(
                    TenantedCosmosDbNEventStoreFactory.SnapshotsContainerDefinition,
                    new CosmosConfiguration { DatabaseName = "workflowspecs", DisableTenantIdPrefix = true });
            }

            var tenant = new Tenant(
                RootTenant.RootTenantId.CreateChildId(Guid.NewGuid()),
                "Marain.Workflow.Specs test run",
                propertyBagFactory.Create(newConfig)) as ITenant;

            scenarioContext.Set(tenant);
        }

        [AfterScenario("perScenarioContainer", "usingCosmosDbNEventStore")]
        public static async Task TearDownCosmosEventStoreContainers(ScenarioContext scenarioContext)
        {
            IServiceProvider serviceProvider = ContainerBindings.GetServiceProvider(scenarioContext);
            ITenantCosmosContainerFactory containerFactory = serviceProvider.GetRequiredService<ITenantCosmosContainerFactory>();

            ITenant tenant = scenarioContext.Get<ITenant>();

            await scenarioContext.RunAndStoreExceptionsAsync(
                async () =>
                {
                    Container container = await containerFactory.GetContainerForTenantAsync(tenant, TenantedCosmosDbNEventStoreFactory.EventsContainerDefinition).ConfigureAwait(false);
                    await container.DeleteContainerAsync().ConfigureAwait(false);
                }).ConfigureAwait(false);

            await scenarioContext.RunAndStoreExceptionsAsync(
                async () =>
                {
                    Container container = await containerFactory.GetContainerForTenantAsync(tenant, TenantedCosmosDbNEventStoreFactory.SnapshotsContainerDefinition).ConfigureAwait(false);
                    await container.DeleteContainerAsync().ConfigureAwait(false);
                }).ConfigureAwait(false);
        }

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

                    string azureServicesAuthConnectionString = root["AzureServicesAuthConnectionString"];

                    services.AddSingleton(root);

                    services.AddLogging();

                    services.AddRootTenant();
                    services.AddInMemoryTenantProvider();

                    services.AddJsonSerializerSettings();

                    services.AddTenantCosmosContainerFactory(new TenantCosmosContainerFactoryOptions
                    {
                        AzureServicesAuthConnectionString = azureServicesAuthConnectionString,
                    });

                    services.AddInMemoryWorkflowTriggerQueue();
                    services.AddInMemoryLeasing();

                    services.RegisterCoreWorkflowContentTypes();
                    services.AddTenantedWorkflowEngineFactory();

                    if (featureContext.FeatureInfo.Tags.Any(t => t == "useCosmosStores"))
                    {
                        services.AddTenantedAzureCosmosWorkflowStore();

                        // TODO: Add workflow instance store.
                        // services.AddTenantedAzureCosmosWorkflowInstanceStore();
                    }
                    else if (featureContext.FeatureInfo.Tags.Any(t => t == "useAzureBlobStore"))
                    {
                        services.AddTenantedBlobWorkflowStore();

                        // We don't yet have a blob implementation of the instance store.
                        services.AddSingleton<ITenantedWorkflowInstanceStoreFactory, FakeTenantedWorkflowInstanceStoreFactory>();
                    }

                    services.AddContent(factory => factory.RegisterTestContentTypes());

                    services.AddSingleton<DataCatalogItemRepositoryFactory>();

                    services.AddSingleton<IServiceIdentityTokenSource, FakeServiceIdentityTokenSource>();
                });
        }
    }
}