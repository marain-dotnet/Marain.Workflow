// <copyright file="WorkflowContainerBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Specs.Bindings
{
    using System;
    using System.Linq;

    using Corvus.Identity.ClientAuthentication.Azure;
    using Corvus.Testing.SpecFlow;
    using Marain.Workflows.Specs.TestObjects;
    using Marain.Workflows.Specs.TestObjects.Subjects;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;

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

                    services.AddInMemoryTenantProvider();

                    services.AddJsonNetSerializerSettingsProvider();
                    services.AddJsonNetPropertyBag();
                    services.AddJsonNetCultureInfoConverter();
                    services.AddJsonNetDateTimeOffsetToIso8601AndUnixTimeConverter();
                    services.AddSingleton<JsonConverter>(new StringEnumConverter(new CamelCaseNamingStrategy()));
                    services.AddCosmosClientBuilderWithNewtonsoftJsonIntegration();

                    // Even non-cosmos tests depend on ICosmosContainerSourceWithTenantLegacyTransition
                    // because the various 'data catalog' parts of the test use a Cosmos DB.
                    services.AddTenantCosmosContainerFactory();
                    services.AddCosmosContainerV2ToV3Transition();

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
                    else if (featureContext.FeatureInfo.Tags.Any(t => t == "useAzureBlobStore"))
                    {
                        services.AddTenantedBlobWorkflowStore();

                        // We don't yet have a blob implementation of the instance store.
                        services.AddSingleton<ITenantedWorkflowInstanceStoreFactory, FakeTenantedWorkflowInstanceStoreFactory>();
                    }

                    services.AddContent(factory => factory.RegisterTestContentTypes());

                    services.AddSingleton<DataCatalogItemRepositoryFactory>();

                    // Faking out the service identity is slightly awkward because in some cases we need
                    // it to work for real (so we can read Azure Key Vault secrets in configuration)
                    // but some tests need to provide fake results. So we spin up a second container
                    // with a real service identity, and put a wrapper around that into the container
                    // that everything is really using.
                    ServiceCollection containerForRealTokenProvider = new();
                    string azureServicesAuthConnectionString = root["AzureServicesAuthConnectionString"];
                    containerForRealTokenProvider
                        .AddServiceIdentityAzureTokenCredentialSourceFromLegacyConnectionString(azureServicesAuthConnectionString);
                    IServiceProvider spForRealTokenProvider = containerForRealTokenProvider.BuildServiceProvider();
                    IServiceIdentityAzureTokenCredentialSource realTokenProvider = spForRealTokenProvider.GetRequiredService<IServiceIdentityAzureTokenCredentialSource>();

                    // We do this to get the IServiceIdentityAccessTokenSource, because there isn't an easy way to get that
                    // but our adapter as the IServiceIdentityAzureTokenCredentialSource.
                    services.AddServiceIdentityAzureTokenCredentialSourceFromLegacyConnectionString(azureServicesAuthConnectionString);

                    // And now we replace the IServiceIdentityAzureTokenCredentialSource with our adapter.
                    services.AddSingleton<IServiceIdentityAzureTokenCredentialSource>(new FakeServiceIdentityTokenSource(realTokenProvider));
                });
        }
    }
}