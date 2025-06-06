// <copyright file="WebRequestSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.Specs.Steps
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    using Corvus.Extensions.Json;
    using Corvus.Testing.ReqnRoll;

    using Marain.TenantManagement.Testing;
    using Marain.Workflows.Api.Specs.Bindings;

    using Microsoft.Extensions.DependencyInjection;

    using Newtonsoft.Json;

    using NUnit.Framework;
    using Reqnroll;

    [Binding]
    public class WebRequestSteps
    {
        private readonly HttpClient http = new();
        private readonly ScenarioContext context;
        private readonly FeatureContext featureContext;
        private readonly TransientTenantManager transientTenantManager;

        public WebRequestSteps(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            this.context = scenarioContext;
            this.featureContext = featureContext;
            this.transientTenantManager = TransientTenantManager.GetInstance(featureContext);
        }

        [Then("I should have received a (.*) status code from the HTTP request")]
        public void ThenIShouldHaveReceivedAStatusCodeFromTheHTTPRequest(int code)
        {
            this.ThenIShouldHaveReceivedStatusCodesFromTheHTTPRequests(1, code);
        }

        [Then("I should have received (.*) (.*) status codes from the HTTP requests")]
        public void ThenIShouldHaveReceivedStatusCodesFromTheHTTPRequests(int count, int code)
        {
            List<int> codes = this.context.Get<List<int>>("HttpResponses");
            IEnumerable<int> responses = codes.Where(x => x == code);
            IEnumerable<string> allCodes = codes.GroupBy(x => x).Select(x => $"{x.Key} ({x.Count()})");

            Assert.GreaterOrEqual(responses.Count(), count, $"Did not receive the required number of {code} status codes. Received the following: {string.Join(", ", allCodes)}");
        }

        [When("I get the workflow engine path '(.*)'")]
        public async Task WhenIGetTheWorkflowEnginePath(string path)
        {
            string url = WorkflowFunctionBindings.EngineHostBaseUrl + path;
            url = url.Replace("{tenantId}", this.transientTenantManager.PrimaryTransientClient.Id);

            HttpRequestMessage request = new(HttpMethod.Get, url);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (!this.context.ContainsKey("HttpResponses"))
            {
                this.context.Add("HttpResponses", new List<int>());
            }

            HttpResponseMessage response = await this.http.SendAsync(request).ConfigureAwait(false);

            this.context.Get<List<int>>("HttpResponses").Add((int)response.StatusCode);
            this.context.Set(response);

            using Stream responseStream = response.Content.ReadAsStream();
            using var responseReader = new StreamReader(responseStream);
            string responseBody = responseReader.ReadToEnd();
            this.context.Set(responseBody, "ResponseBody");
        }

        [When("I post the object called '(.*)' to the workflow engine path '(.*)'")]
        public async Task WhenIPostTheObjectCalledToTheWorkflowEnginePath(string instanceName, string path)
        {
            string url = WorkflowFunctionBindings.EngineHostBaseUrl + path;
            await this.PostContextObjectToEndpoint(instanceName, url).ConfigureAwait(false);
        }

        [When("I post the object called '(.*)' to the workflow message processing path '(.*)'")]
        public async Task WhenIPostTheObjectCalledToTheWorkflowMessageProcessingPath(string instanceName, string path)
        {
            string url = WorkflowFunctionBindings.MessageProcessingHostBaseUrl + path;
            await this.PostContextObjectToEndpoint(instanceName, url).ConfigureAwait(false);
        }

        [Given("I have posted the workflow called '(.*)' to the workflow engine path '(.*)'")]
        [When("I post the workflow called '(.*)' to the workflow engine path '(.*)'")]
        public async Task WhenIPostTheWorkflowCalledToTheWorkflowEnginePath(string workflowName, string path)
        {
            string url = WorkflowFunctionBindings.EngineHostBaseUrl + path;
            url = url.Replace("{tenantId}", this.transientTenantManager.PrimaryTransientClient.Id);
            Workflow workflow = this.context.Get<Workflow>(workflowName);

            // When POSTing, we shouldn't send an etag.
            workflow.ETag = null;
            await this.SendObjectToEndpoint(workflow, url, HttpMethod.Post).ConfigureAwait(false);
        }

        [When("I put the workflow called '(.*)' to the workflow engine path '(.*)'")]
        public async Task WhenIPutTheWorkflowCalledToTheWorkflowEnginePath(string workflowName, string path)
        {
            string url = WorkflowFunctionBindings.EngineHostBaseUrl + path;
            url = url.Replace("{tenantId}", this.transientTenantManager.PrimaryTransientClient.Id);
            Workflow workflow = this.context.Get<Workflow>(workflowName);
            await this.SendObjectToEndpoint(workflow, url, HttpMethod.Put).ConfigureAwait(false);
        }

        [When("I put the workflow called '(.*)' to the workflow engine path '(.*)' with an If-Match header value from the etag of the workflow")]
        public async Task WhenIPutTheWorkflowCalledToTheWorkflowEnginePathWithAnIfMatchHeaderValueFromTheEtagOfTheWorkflow(string workflowName, string path)
        {
            string url = WorkflowFunctionBindings.EngineHostBaseUrl + path;
            url = url.Replace("{tenantId}", this.transientTenantManager.PrimaryTransientClient.Id);
            Workflow workflow = this.context.Get<Workflow>(workflowName);

            await this.SendObjectToEndpoint(workflow, url, HttpMethod.Put, setHeaders: msg => msg.Headers.IfMatch.Add(new EntityTagHeaderValue(workflow.ETag))).ConfigureAwait(false);
        }

        [When("I put the workflow called '(.*)' to the workflow engine path '(.*)' with '(.*)' as the If-Match header value")]
        public async Task WhenIPutTheWorkflowCalledToTheWorkflowEnginePathWithAnIfMatchHeaderValueOf(string workflowName, string path, string ifMatchHeaderValue)
        {
            string url = WorkflowFunctionBindings.EngineHostBaseUrl + path;
            url = url.Replace("{tenantId}", this.transientTenantManager.PrimaryTransientClient.Id);
            Workflow workflow = this.context.Get<Workflow>(workflowName);

            await this.SendObjectToEndpoint(workflow, url, HttpMethod.Put, setHeaders: msg => msg.Headers.IfMatch.Add(new EntityTagHeaderValue(ifMatchHeaderValue))).ConfigureAwait(false);
        }

        private async Task PostContextObjectToEndpoint(string instanceName, string url)
        {
            url = url.Replace("{tenantId}", this.transientTenantManager.PrimaryTransientClient.Id);

            foreach (object obj in this.context.Get<IEnumerable<object>>(instanceName))
            {
                await this.SendObjectToEndpoint(obj, url, HttpMethod.Post).ConfigureAwait(false);
            }
        }

        private async Task SendObjectToEndpoint(object instance, string url, HttpMethod method, Action<HttpRequestMessage> setHeaders = null)
        {
            IJsonSerializerSettingsProvider serializationSettingsProvider = ContainerBindings.GetServiceProvider(this.featureContext).GetRequiredService<IJsonSerializerSettingsProvider>();
            string body = JsonConvert.SerializeObject(instance, serializationSettingsProvider.Instance);
            var address = new Uri(url);

            HttpRequestMessage request = new(method, address)
            {
                Content = new StringContent(body, encoding: null, mediaType: "application/json"),
            };

            setHeaders?.Invoke(request);

            if (!this.context.ContainsKey("HttpResponses"))
            {
                this.context.Add("HttpResponses", new List<int>());
            }

            HttpResponseMessage response = await this.http.SendAsync(request).ConfigureAwait(false);

            this.context.Get<List<int>>("HttpResponses").Add((int)response.StatusCode);
        }
    }
}