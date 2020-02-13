// <copyright file="Startup.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

[assembly: Microsoft.Azure.WebJobs.Hosting.WebJobsStartup(typeof(Marain.Workflows.Api.EngineHost.Startup))]

namespace Marain.Workflows.Api.EngineHost
{
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Hosting;
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

            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Debug);
            });

            services.AddTenantedWorkflowEngineApi(config => config.Documents.AddSwaggerEndpoint());
        }
    }
}
