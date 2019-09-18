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
    using System.Threading.Tasks;
    using Corvus.Extensions.Json;
    using Corvus.Identity.ManagedServiceIdentity.ClientAuthentication;
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

        private static readonly HttpClient HttpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(300) };
        private readonly IServiceIdentityTokenSource serviceIdentityTokenSource;
        private readonly IJsonSerializerSettingsProvider serializerSettingsProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvokeExternalServiceAction"/> class.
        /// </summary>
        /// <param name="serviceIdentityTokenSource">The identity token source.</param>
        /// <param name="serializerSettingsProvider">The serializer settings provider.</param>
        public InvokeExternalServiceAction(IServiceIdentityTokenSource serviceIdentityTokenSource, IJsonSerializerSettingsProvider serializerSettingsProvider)
        {
            this.serviceIdentityTokenSource = serviceIdentityTokenSource ?? throw new ArgumentNullException(nameof(serviceIdentityTokenSource));
            this.serializerSettingsProvider = serializerSettingsProvider ?? throw new ArgumentNullException(nameof(serializerSettingsProvider));
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
        public async Task ExecuteAsync(WorkflowInstance instance, IWorkflowTrigger trigger)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, this.ExternalUrl);

            if (this.AuthenticateWithManagedServiceIdentity)
            {
                string token = await this.serviceIdentityTokenSource.GetAccessToken(this.MsiAuthenticationResource).ConfigureAwait(false);

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
                throw new ExternalServiceInvocationException(this.ContentType, this.Id, instance.Id, trigger.Id, response.StatusCode, response.ReasonPhrase);
            }
        }
    }
}
