// <copyright file="WorkflowFunctionsContainerBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflow.Api.Specs.Bindings
{
    using Corvus.Azure.Cosmos.Tenancy;
    using Corvus.Azure.Storage.Tenancy;
    using Corvus.Leasing;
    using Corvus.SpecFlow.Extensions;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using TechTalk.SpecFlow;

    /// <summary>
    /// Bindings to set up the test container with workflow services so that test setup/teardown can be performed.
    /// </summary>
    [Binding]
    public static class WorkflowFunctionsContainerBindings
    {
        /// <summary>
        /// Setup the endjin container for a feature.
        /// </summary>
        /// <param name="featureContext">The feature context for the current feature.</param>
        /// <remarks>We expect features run in parallel to be executing in separate app domains.</remarks>
        [BeforeFeature("@perFeatureContainer", Order = ContainerBeforeFeatureOrder.PopulateServiceCollection)]
        public static void SetupFeature(FeatureContext featureContext)
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
                    services.AddJsonSerializerSettings();

                    services.AddLogging();

                    string azureServicesAuthConnectionString = root["AzureServicesAuthConnectionString"];

                    TenantCloudBlobContainerFactoryOptions blobStorageConfiguration = root.GetSection("TenantCloudBlobContainerFactoryOptions").Get<TenantCloudBlobContainerFactoryOptions>()
                        ?? new TenantCloudBlobContainerFactoryOptions();
                    if (blobStorageConfiguration.RootTenantBlobStorageConfiguration == null)
                    {
                        blobStorageConfiguration.RootTenantBlobStorageConfiguration = new BlobStorageConfiguration();
                    }

                    services.AddTenantCloudBlobContainerFactory(blobStorageConfiguration);
                    services.AddTenantProviderBlobStore();

                    TenantCosmosContainerFactoryOptions cosmosConfiguration = root.GetSection("TenantCosmosContainerFactoryOptions").Get<TenantCosmosContainerFactoryOptions>()
                        ?? new TenantCosmosContainerFactoryOptions();
                    if (cosmosConfiguration.RootTenantCosmosConfiguration == null)
                    {
                        cosmosConfiguration.RootTenantCosmosConfiguration = new CosmosConfiguration();
                    }

                    services.AddTenantCosmosContainerFactory(cosmosConfiguration);

                    services.AddTenantedWorkflowEngineFactory();
                    services.AddTenantedAzureCosmosWorkflowStore();
                    services.AddTenantedAzureCosmosWorkflowInstanceStore();

                    services.RegisterCoreWorkflowContentTypes();

                    services.AddAzureLeasing(svc =>
                    {
                        IConfiguration config = svc.GetRequiredService<IConfiguration>();
                        return new AzureLeaseProviderOptions
                        {
                            StorageAccountConnectionString = config["LeasingStorageAccountConnectionString"],
                        };
                    });
                });
        }
    }
}
