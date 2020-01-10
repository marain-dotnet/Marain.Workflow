// <copyright file="OpenApiWebHostStartup.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Corvus.SpecFlow.Extensions.SelfHostedOpenApiFunctionManagement
{
    using System;
    using Menes;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Abstractions;
    using Microsoft.AspNetCore.Routing;
    using Microsoft.Azure.WebJobs.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;

    /// <summary>
    /// Startup class used with <see cref="IWebHostBuilder"/> to initialise a webhost using an <see cref="IWebJobsStartup"/>
    /// implementation from a functions app.
    /// </summary>
    /// <typeparam name="TWebJobStartup">
    /// The type of the Startup class to use to configure the <see cref="IServiceCollection"/>.
    /// </typeparam>
    internal class OpenApiWebHostStartup<TWebJobStartup>
        where TWebJobStartup : IWebJobsStartup, new()
    {
        /// <summary>
        /// Configures the <see cref="IServiceCollection"/>, adding MVC routing and then handing off to the specified
        /// Startup class to add further required services.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add to.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting();

            this.AddConfiguration(services);

            var builder = new WebJobBuilder(services);
            var targetStartup = new TWebJobStartup();
            targetStartup.Configure(builder);
        }

        /// <summary>
        /// Configures the function host, adding a catch-all route that then hands off to Menes to process the request.
        /// </summary>
        /// <param name="app">The <see cref="IApplicationBuilder"/> to configure.</param>
        public void Configure(IApplicationBuilder app)
        {
            var openApiRouteHandler = new RouteHandler(
                async context =>
                {
                    try
                    {
                        IOpenApiHost<HttpRequest, IActionResult> handler = context.RequestServices.GetRequiredService<IOpenApiHost<HttpRequest, IActionResult>>();
                        IActionResult result = await handler.HandleRequestAsync(context.Request.Path, context.Request.Method, context.Request, context).ConfigureAwait(false);
                        var actionContext = new ActionContext(context, context.GetRouteData(), new ActionDescriptor());
                        await result.ExecuteResultAsync(actionContext).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        Assert.Fail(ex.ToString());
                    }
                });

            var routeBuilder = new RouteBuilder(app, openApiRouteHandler);

            routeBuilder.MapRoute("CatchAll", "{*path}");
            IRouter router = routeBuilder.Build();
            app.UseRouter(router);
        }

        /// <summary>
        /// Adds configuration to the collection. In the Functions world, this is done automatically; for
        /// our in memory hosting, we need to do it manually.
        /// </summary>
        /// <param name="services">The service collection to add to.</param>
        private void AddConfiguration(IServiceCollection services)
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);

            IConfiguration root = configurationBuilder.Build();

            services.AddSingleton(root);
        }
    }
}
