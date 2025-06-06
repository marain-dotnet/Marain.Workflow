// <copyright file="WorkflowFunctionsContainerBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.Specs.Bindings
{
    using Corvus.Leasing;
    using Corvus.Testing.ReqnRoll;
    using Marain.Services;
    using Marain.Tenancy.Client;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Serialization;
    using Reqnroll;

    /// <summary>
    /// Bindings to set up the test container with workflow services so that test setup/teardown can be performed.
    /// </summary>
    [Binding]
    public static class WorkflowFunctionsContainerBindings
    {
        /// <summary>
        /// Set up the endjin container for a feature.
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

                    services.AddJsonNetSerializerSettingsProvider();
                    services.AddJsonNetPropertyBag();
                    services.AddJsonNetCultureInfoConverter();
                    services.AddJsonNetDateTimeOffsetToIso8601AndUnixTimeConverter();
                    services.AddSingleton<JsonConverter>(new StringEnumConverter(new CamelCaseNamingStrategy()));

                    services.AddLogging();

                    string azureServicesAuthConnectionString = root["AzureServicesAuthConnectionString"];

                    services.AddServiceIdentityAzureTokenCredentialSourceFromLegacyConnectionString(azureServicesAuthConnectionString);
                    services.AddMicrosoftRestAdapterForServiceIdentityAccessTokenSource();

                    services.AddSingleton(sp => sp.GetRequiredService<IConfiguration>().GetSection("TenancyClient").Get<TenancyClientOptions>());

                    services.AddSingleton(new MarainServiceConfiguration());
                    services.AddMarainServicesTenancy();

                    // Add tenant provider with caching disabled to prevent issues with the TransientTenantManager
                    // creating and updating tenants.
                    services.AddTenantProviderServiceClient(false);

                    // Workflow definitions get stored in blob storage
                    services.AddTenantedBlobWorkflowStore();

                    // Workflow instances get stored in CosmosDB
                    services.AddCosmosClientBuilderWithNewtonsoftJsonIntegration();
                    services.AddTenantedAzureCosmosWorkflowInstanceStore();

                    services.AddTenantedWorkflowEngineFactory();

                    services.RegisterCoreWorkflowContentTypes();

                    services.AddMarainTenantManagementForBlobStorage();
                    services.AddMarainTenantManagementForCosmosDb();

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