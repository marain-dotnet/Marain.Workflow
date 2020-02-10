// <copyright file="HttpStarter.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.MessageProcessingHost.Starters
{
    using System.Threading.Tasks;
    using Marain.Workflows.Api.MessageProcessingHost.OpenApi;
    using Menes;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.DurableTask;
    using Microsoft.Azure.WebJobs.Extensions.Http;

    /// <summary>
    /// Endpoint for Http requests that start durable orchestrations.
    /// </summary>
    public class HttpStarter
    {
        private readonly IOpenApiHost<HttpRequest, IActionResult> host;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpStarter"/> class.
        /// </summary>
        /// <param name="host">The OpenApi host.</param>
        public HttpStarter(IOpenApiHost<HttpRequest, IActionResult> host)
        {
            this.host = host;
        }

        /// <summary>
        /// Http entry point.
        /// </summary>
        /// <param name="req">The <see cref="HttpRequest"/>.</param>
        /// <param name="orchestrationClient">The durable functions orchestration client.</param>
        /// <param name="executionContext">The context for the function execution.</param>
        /// <returns>An action result which comes from executing the function.</returns>
        [FunctionName("HttpStarter")]
        public Task<IActionResult> RunAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "{*path}")] HttpRequest req,
            [DurableClient] IDurableOrchestrationClient orchestrationClient,
            ExecutionContext executionContext)
        {
            var additionalParameters = new DurableFunctionsOpenApiContextAdditionalProperties
            {
                ExecutionContext = executionContext,
                OrchestrationClient = orchestrationClient,
            };

            return this.host.HandleRequestAsync(req.Path, req.Method, req, additionalParameters);
        }
    }
}
