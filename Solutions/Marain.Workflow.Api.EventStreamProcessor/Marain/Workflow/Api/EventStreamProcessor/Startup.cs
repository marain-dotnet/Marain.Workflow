// <copyright file="Startup.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

[assembly: Microsoft.Azure.WebJobs.Hosting.WebJobsStartup(typeof(Marain.Workflows.Api.EventStreamProcessor.Startup))]

namespace Marain.Workflows.Api.EventStreamProcessor
{
    using Corvus.Azure.Cosmos.Tenancy;
    using Marain.Tenancy.Client;
    using Marain.Workflow.Api.EventStreamProcessor.Configuration;
    using Microsoft.Azure.Functions.Extensions.DependencyInjection;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Startup code for the Function.
    /// </summary>
    public class Startup : FunctionsStartup
    {
        /// <inheritdoc/>
        public override void Configure(IFunctionsHostBuilder builder)
        {
            IServiceCollection services = builder.Services;

            services.AddApplicationInsightsInstrumentationTelemetry();
            services.AddLogging();

            AddConfiguration(services);

            AddTenancy(services);

            AddCosmosBackedWorkflowInstanceStorage(services);
        }

        private static void AddConfiguration(IServiceCollection services)
        {
            services.AddSingleton(
                sp =>
                {
                    IConfiguration config = sp.GetRequiredService<IConfiguration>();
                    return config.GetSection("EventStreamProcessorConfiguration").Get<EventStreamProcessorConfiguration>()
                        ?? new EventStreamProcessorConfiguration();
                });
        }

        /// <summary>
        /// Adds tenancy services.
        /// </summary>
        /// <param name="services">The service collection.</param>
        private static void AddTenancy(
            IServiceCollection services)
        {
            services.AddTenantProviderServiceClient();

            services.AddSingleton(
                sp =>
                {
                    IConfiguration config = sp.GetRequiredService<IConfiguration>();
                    return config.GetSection("TenancyClient").Get<TenancyClientOptions>()
                        ?? new TenancyClientOptions();
                });

            services.AddMarainServiceConfiguration();
            services.AddMarainServicesTenancy();
        }

        private static void AddCosmosBackedWorkflowInstanceStorage(
            IServiceCollection services)
        {
            services.AddTenantCosmosContainerFactory(
                sp =>
                {
                    IConfiguration config = sp.GetRequiredService<IConfiguration>();
                    return new TenantCosmosContainerFactoryOptions
                    {
                        AzureServicesAuthConnectionString = config["AzureServicesAuthConnectionString"],
                    };
                });

            services.AddCosmosDbWorkflowInstanceEventStore();
        }
    }
}