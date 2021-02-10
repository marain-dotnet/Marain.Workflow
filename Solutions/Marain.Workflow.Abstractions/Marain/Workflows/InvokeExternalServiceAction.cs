// <copyright file="InvokeExternalServiceAction.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Corvus.Extensions.Json;
    using Corvus.Identity.ManagedServiceIdentity.ClientAuthentication;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;

    /// <summary>
    /// A workflow action that POSTs to an external service.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This action performs an HTTP POST when executed to the endpoint specified by
    /// <see cref="ExternalUrl"/>. This enables workflow users to supply 'webhooks'.
    /// </para>
    /// <para>
    /// The content type of the request that this action makes is defined by <see cref="ExternalServiceWorkflowRequest"/>.
    /// </para>
    /// </remarks>
    public class InvokeExternalServiceAction : IWorkflowAction
    {
        /// <summary>
        /// The content type that will be used when serializing/deserializing.
        /// </summary>
        public const string RegisteredContentType = "application/vnd.marain.workflows.invokeexternalserviceaction";

        // Although not recommended, it is possible that external requests could take quite some time. Care should be
        // taken with this; depending on the hosting option selected, the hosting environment may place its own limits
        // on the time taken for total request execution time. For example, on the Azure Functions consumption plan,
        // the maximum function execution time is 10 minutes. However, other hosting environments permit longer
        // execution time, and we don't want to impose unnecessary limitations on external service execution time.
        private static readonly HttpClient HttpClient = new HttpClient { Timeout = Timeout.InfiniteTimeSpan };
        private readonly IServiceIdentityTokenSource serviceIdentityTokenSource;
        private readonly IJsonSerializerSettingsProvider serializerSettingsProvider;
        private readonly ILogger<InvokeExternalServiceAction> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvokeExternalServiceAction"/> class.
        /// </summary>
        /// <param name="serviceIdentityTokenSource">The token source to use when authenticating to third party services.</param>
        /// <param name="serializerSettingsProvider">The serialization settings to use when serializing requests.</param>
        /// <param name="logger">The logger.</param>
        public InvokeExternalServiceAction(
            IServiceIdentityTokenSource serviceIdentityTokenSource,
            IJsonSerializerSettingsProvider serializerSettingsProvider,
            ILogger<InvokeExternalServiceAction> logger)
        {
            this.serviceIdentityTokenSource =
                serviceIdentityTokenSource ?? throw new ArgumentNullException(nameof(serviceIdentityTokenSource));
            this.serializerSettingsProvider =
                serializerSettingsProvider ?? throw new ArgumentNullException(nameof(serializerSettingsProvider));
            this.logger =
                logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public string ContentType => RegisteredContentType;

        /// <inheritdoc />
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the URL of the external service that this action will POST to when executed.
        /// </summary>
        public string ExternalUrl { get; set; }

        /// <summary>
        /// Gets or sets a list of the context items to include in the body of the POST.
        /// </summary>
        public IEnumerable<string> ContextItemsToInclude { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether requests to the external service should be
        /// authenticated using the managed service identity of the hosting service.
        /// </summary>
        public bool AuthenticateWithManagedServiceIdentity { get; set; }

        /// <summary>
        /// Gets or sets the resource for which to authenticate when
        /// <see cref="AuthenticateWithManagedServiceIdentity"/> is true.
        /// </summary>
        /// <remarks>
        /// <para>
        /// When the external service hosted in Azure and has Easy Auth enabled, this will be the
        /// GUID of the AAD App that Azure created to represent that application.
        /// </para>
        /// </remarks>
        public string MsiAuthenticationResource { get; set; }

        /// <inheritdoc />
        public async Task<WorkflowActionResult> ExecuteAsync(WorkflowInstance instance, IWorkflowTrigger trigger)
        {
            HttpRequestMessage request =
                await this.PrepareRequestAsync(instance, trigger).ConfigureAwait(false);

            HttpResponseMessage httpResponse =
                await this.ExecuteRequestAsync(request, instance, trigger).ConfigureAwait(false);

            return await this.ProcessResponseAsync(httpResponse, instance, trigger).ConfigureAwait(false);
        }

        private async Task<WorkflowActionResult> ProcessResponseAsync(
            HttpResponseMessage httpResponse,
            WorkflowInstance instance,
            IWorkflowTrigger trigger)
        {
            // Get the response body and see if it's useful
            string responseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (!string.IsNullOrEmpty(responseContent))
            {
                this.logger.LogDebug(
                    "Processing response for workflow instance '{workflowInstanceId} from call to external URL '{externalUrl}' resulting from trigger '{triggerId}'",
                    instance.Id,
                    this.ExternalUrl,
                    trigger?.Id ?? "{no trigger}");

                // Read and process the response.
                ExternalServiceWorkflowResponse response = JsonConvert.DeserializeObject<ExternalServiceWorkflowResponse>(
                    responseContent,
                    this.serializerSettingsProvider.Instance);

                return new WorkflowActionResult(response.ContextValuesToSetOrAdd, response.ContextValuesToRemove);
            }
            else
            {
                this.logger.LogDebug(
                    "Request to external URL '{externalUrl}' did not return any response.",
                    instance.Id,
                    this.ExternalUrl);

                return WorkflowActionResult.Empty;
            }
        }

        private async Task<HttpRequestMessage> PrepareRequestAsync(
            WorkflowInstance instance,
            IWorkflowTrigger trigger)
        {
            this.logger.LogDebug(
                "Initialising request for workflow instance '{workflowInstanceId} to external URL '{externalUrl}' resulting from trigger '{triggerId}'",
                instance.Id,
                this.ExternalUrl,
                trigger?.Id ?? "{no trigger}");

            var request = new HttpRequestMessage(HttpMethod.Post, this.ExternalUrl);

            if (this.AuthenticateWithManagedServiceIdentity)
            {
                string token = await this.serviceIdentityTokenSource.GetAccessToken(
                    this.MsiAuthenticationResource).ConfigureAwait(false);

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var requestBody = new ExternalServiceWorkflowRequest
            {
                WorkflowId = instance.WorkflowId,
                WorkflowInstanceId = instance.Id,
                WorkflowInstanceCurrentStateId = instance.StateId,
                WorkflowInstanceCurrentStatus = instance.Status.ToString(),
                InvokedById = this.Id,
                Trigger = trigger,
            };

            if (this.ContextItemsToInclude?.Any() == true)
            {
                requestBody.ContextProperties = instance.Context
                    .Where(kv => this.ContextItemsToInclude.Contains(kv.Key))
                    .ToDictionary(kv => kv.Key, kv => kv.Value);

                this.logger.LogDebug($"Including context keys {string.Join(',', requestBody.ContextProperties.Keys)}");
            }

            request.Content = new StringContent(
                JsonConvert.SerializeObject(requestBody, this.serializerSettingsProvider.Instance),
                Encoding.UTF8,
                ExternalServiceWorkflowRequest.RegisteredContentType);

            return request;
        }

        private async Task<HttpResponseMessage> ExecuteRequestAsync(
            HttpRequestMessage request,
            WorkflowInstance instance,
            IWorkflowTrigger trigger)
        {
            // TODO: Add retry logic - https://github.com/marain-dotnet/Marain.Workflow/issues/104
            HttpResponseMessage httpResponse = await HttpClient.SendAsync(request).ConfigureAwait(false);

            if (!httpResponse.IsSuccessStatusCode)
            {
                throw new ExternalServiceInvocationException(
                    this.ContentType,
                    this.Id,
                    instance.Id,
                    trigger?.Id ?? "{no trigger}",
                    httpResponse.StatusCode,
                    httpResponse.ReasonPhrase);
            }

            return httpResponse;
        }
    }
}
