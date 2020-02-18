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

                    BlobStorageConfiguration blobStorageConfiguration = root.GetSection("ROOTTENANTBLOBSTORAGECONFIGURATIONOPTIONS").Get<BlobStorageConfiguration>()
                        ?? new BlobStorageConfiguration();

                    services.AddTenantCloudBlobContainerFactory(new TenantCloudBlobContainerFactoryOptions
                    {
                        AzureServicesAuthConnectionString = azureServicesAuthConnectionString,
                        RootTenantBlobStorageConfiguration = blobStorageConfiguration,
                    });
                    services.AddTenantProviderBlobStore();

                    CosmosConfiguration cosmosConfiguration = root.GetSection("ROOTTENANTCOSMOSCONFIGURATIONOPTIONS").Get<CosmosConfiguration>()
                        ?? new CosmosConfiguration();

                    services.AddTenantCosmosContainerFactory(new TenantCosmosContainerFactoryOptions
                    {
                        AzureServicesAuthConnectionString = azureServicesAuthConnectionString,
                        RootTenantCosmosConfiguration = cosmosConfiguration,
                    });

                    services.AddTenantCosmosContainerFactory(sp =>
                    {
                        IConfiguration config = sp.GetRequiredService<IConfiguration>();

                        return new TenantCosmosContainerFactoryOptions
                        {
                            AzureServicesAuthConnectionString = config["AzureServicesAuthConnectionString"],
                        };
                    });

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
