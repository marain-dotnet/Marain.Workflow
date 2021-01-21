// <copyright file="WorkflowContainerBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Specs.Bindings
{
    using System.Linq;
    using Corvus.Azure.Cosmos.Tenancy;
    using Corvus.Identity.ManagedServiceIdentity.ClientAuthentication;
    using Corvus.Testing.SpecFlow;
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
        /// Initialises the content type factory for use with specs that are more "unit test" in nature.
        /// </summary>
        /// <param name="scenarioContext">The current scenario context.</param>
        [BeforeScenario("@perScenarioContainer", Order = ContainerBeforeScenarioOrder.PopulateServiceCollection)]
        public static void InitializeContentFactory(ScenarioContext scenarioContext)
        {
            ContainerBindings.ConfigureServices(
                scenarioContext,
                services =>
                {
                    services.AddJsonSerializerSettings();
                    services.RegisterCoreWorkflowContentTypes();
                    services.AddContent(factory => factory.RegisterTestContentTypes());
                });
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
                        services.AddTenantedAzureCosmosWorkflowInstanceStore();
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