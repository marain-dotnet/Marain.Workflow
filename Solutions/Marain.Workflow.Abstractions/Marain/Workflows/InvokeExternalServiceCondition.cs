// <copyright file="InvokeExternalServiceCondition.cs" company="Endjin Limited">
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
    using System.Threading.Tasks;
    using Corvus.Extensions.Json;
    using Corvus.Identity.ManagedServiceIdentity.ClientAuthentication;
    using Newtonsoft.Json;

    /// <summary>
    /// A workflow condition that makes an HTTP request to an external service to determine the
    /// condition result.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This condition performs either an HTTP POST or an HTTP GET when executed to the endpoint
    /// specified by <see cref="ExternalUrl"/>. The body of the response must have content type
    /// <c>application/json</c>, and must contain just a single boolean value, i.e., either
    /// <c>true</c> or <c>false</c>. This value determines the result of
    /// <see cref="EvaluateAsync(WorkflowInstance, IWorkflowTrigger)"/>.
    /// </para>
    /// <para>
    /// If <see cref="HttpMethod"/> is set to <see cref="HttpMethod.Post"/>, the content type of
    /// the request that this action makes is defined by <see cref="ExternalServiceWorkflowRequest"/>.
    /// If using <see cref="HttpMethod.Get"/>, no contextual information will be included in the
    /// HTTP request.
    /// </para>
    /// </remarks>
    public class InvokeExternalServiceCondition : IWorkflowCondition
    {
        /// <summary>
        /// The content type that will be used when serializing/deserializing.
        /// </summary>
        public const string RegisteredContentType = "application/vnd.marain.workflows.invokeexternalservicecondition";

        private static readonly HttpClient HttpClient = new HttpClient();
        private readonly IServiceIdentityTokenSource serviceIdentityTokenSource;
        private readonly IJsonSerializerSettingsProvider serializerSettingsProvider;
        private HttpMethod httpMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvokeExternalServiceAction"/> class.
        /// </summary>
        /// <param name="serviceIdentityTokenSource">The token source to use when authenticating to third party services.</param>
        /// <param name="serializerSettingsProvider">The serialization settings to use when serializing requests.</param>
        public InvokeExternalServiceCondition(
            IServiceIdentityTokenSource serviceIdentityTokenSource,
            IJsonSerializerSettingsProvider serializerSettingsProvider)
        {
            this.serviceIdentityTokenSource =
                serviceIdentityTokenSource ?? throw new ArgumentNullException(nameof(serviceIdentityTokenSource));
            this.serializerSettingsProvider =
                serializerSettingsProvider ?? throw new ArgumentNullException(nameof(serializerSettingsProvider));
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
        /// Gets or sets the HTTP method to use. Must be either <see cref="HttpMethod.Get"/> or
        /// <see cref="HttpMethod.Post"/>.
        /// </summary>
        public HttpMethod HttpMethod
        {
            get => this.httpMethod;

            set
            {
                if (value != HttpMethod.Get && value != HttpMethod.Post)
                {
                    throw new ArgumentException("Only GET and POST are supported");
                }

                this.httpMethod = value;
            }
        }

        /// <summary>
        /// Gets or sets a list of interest to return from <see cref="GetInterests(WorkflowInstance)"/>.
        /// </summary>
        public IEnumerable<string> Interests { get; set; }

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
        public async Task<bool> EvaluateAsync(WorkflowInstance instance, IWorkflowTrigger trigger)
        {
            var request = new HttpRequestMessage(this.HttpMethod, this.ExternalUrl);

            if (this.AuthenticateWithManagedServiceIdentity)
            {
                string token =
                    await this.serviceIdentityTokenSource.GetAccessToken(this.MsiAuthenticationResource).ConfigureAwait(false);

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var responseBody = new ExternalServiceWorkflowRequest
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
                responseBody.ContextProperties = instance.Context
                    .Where(kv => this.ContextItemsToInclude.Contains(kv.Key))
                    .ToDictionary(kv => kv.Key, kv => kv.Value);
            }

            request.Content = new StringContent(
                JsonConvert.SerializeObject(responseBody, this.serializerSettingsProvider.Instance),
                Encoding.UTF8,
                ExternalServiceWorkflowRequest.RegisteredContentType);

            HttpResponseMessage response = await HttpClient.SendAsync(request).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new ExternalServiceInvocationException(
                    this.ContentType,
                    this.Id,
                    instance.Id,
                    trigger?.Id ?? "{no trigger}",
                    response.StatusCode,
                    response.ReasonPhrase);
            }

            string responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return responseString == "true";
        }

        /// <inheritdoc />
        public IEnumerable<string> GetInterests(WorkflowInstance instance) => this.Interests ?? Enumerable.Empty<string>();
    }
}
