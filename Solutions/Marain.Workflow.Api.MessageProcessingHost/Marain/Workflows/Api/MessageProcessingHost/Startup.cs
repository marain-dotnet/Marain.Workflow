// <copyright file="Startup.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

[assembly: Microsoft.Azure.WebJobs.Hosting.WebJobsStartup(typeof(Marain.Workflows.Api.MessageProcessingHost.Shared.Startup))]

namespace Marain.Workflows.Api.MessageProcessingHost.Shared
{
    using System;
    using System.Linq;
    using Marain.Operations.Client.OperationsControl;
    using Marain.Workflows.Api.MessageProcessingHost.OpenApi;
    using Marain.Workflows.Client;
    using Menes;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Startup code for the Function.
    /// </summary>
    public class Startup : IWebJobsStartup
    {
        /// <inheritdoc/>
        public void Configure(IWebJobsBuilder builder)
        {
            IServiceCollection services = builder.Services;

            IConfigurationRoot root = Configure(services);
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Debug);
            });

            var uri = new Uri(root["Operations:ControlServiceBaseUrl"]);
            services.AddOperationsControlClient(uri);

            services.AddMarainWorkflowEngineClient(sp =>
            {
                IConfiguration config = sp.GetRequiredService<IConfiguration>();

                return config.GetSection("Workflow:EngineClient").Get<MarainWorkflowEngineClientOptions>();
            });

            services.AddTenantedWorkflowEngine();
            AddMessageProcessingMenesServices(services);
        }

        /// <summary>
        /// Adds services required by workflow engine API.
        /// </summary>
        /// <param name="services">The service collection.</param>
        /// <returns>The service collection, to enable chaining.</returns>
        private static IServiceCollection AddMessageProcessingMenesServices(
            IServiceCollection services)
        {
            // Verify that these services aren't already present
            Type ingestionServiceType = typeof(IMarainWorkflowMessageIngestion);

            // If any of the OpenApi services are already installed, assume we've already completed installation and return.
            if (services.Any(services => ingestionServiceType.IsAssignableFrom(services.ImplementationType)))
            {
                return services;
            }

            services.AddMarainWorkflowMessageIngestionClient(sp =>
            {
                IConfiguration config = sp.GetRequiredService<IConfiguration>();

                return config.GetSection("Workflow:MessageIngestionClient").Get<MarainWorkflowMessageIngestionClientOptions>();
            });

            services.AddTenantedWorkflowEngine();

            return services;
        }

        private static IConfigurationRoot Configure(IServiceCollection services)
        {
            // TODO: Get rid of this and replace any of our code that depends on it with custom configuration sections.
            // https://github.com/marain-dotnet/Marain.Workflow/issues/45
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot root = configurationBuilder.Build();
            services.AddSingleton(root);
            return root;
        }
    }
}