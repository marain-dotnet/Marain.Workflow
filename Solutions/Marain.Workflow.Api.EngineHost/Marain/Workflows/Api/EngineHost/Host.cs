// <copyright file="Host.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.EngineHost
{
    using System.Threading.Tasks;
    using Menes;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;

    /// <summary>
    /// The host for the functions app.
    /// </summary>
    public class Host
    {
        private readonly IOpenApiHost<HttpRequest, IActionResult> host;

        /// <summary>
        /// Initializes a new instance of the <see cref="Host"/> class.
        /// </summary>
        /// <param name="host">The OpenApi host.</param>
        public Host(IOpenApiHost<HttpRequest, IActionResult> host)
        {
            this.host = host;
        }

        /// <summary>
        /// Azure Functions entry point.
        /// </summary>
        /// <param name="req">The <see cref="HttpRequest"/>.</param>
        /// <param name="executionContext">The context for the function execution.</param>
        /// <returns>An action result which comes from executing the function.</returns>
        [FunctionName("EngineHost-OpenApiHostRoot")]
        public Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "{*path}")]
            HttpRequest req,
            ExecutionContext executionContext)
        {
            return this.host.HandleRequestAsync(req.Path, req.Method, req, new { ExecutionContext = executionContext });
        }
    }
}