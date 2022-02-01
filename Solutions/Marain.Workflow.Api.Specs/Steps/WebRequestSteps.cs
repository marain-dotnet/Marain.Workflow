// <copyright file="WebRequestSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.Specs.Steps
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;

    using Corvus.Extensions.Json;
    using Corvus.Testing.SpecFlow;

    using Marain.TenantManagement.Testing;
    using Marain.Workflows.Api.Specs.Bindings;

    using Microsoft.Extensions.DependencyInjection;

    using Newtonsoft.Json;

    using NUnit.Framework;

    using TechTalk.SpecFlow;

    [Binding]
    public class WebRequestSteps
    {
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
        public void WhenIGetTheWorkflowEnginePath(string path)
        {
            string url = WorkflowFunctionBindings.EngineHostBaseUrl + path;
            url = url.Replace("{tenantId}", this.transientTenantManager.PrimaryTransientClient.Id);

            HttpWebRequest request = WebRequest.CreateHttp(url);
            request.Accept = "application/json";
            request.Method = "GET";

            if (!this.context.ContainsKey("HttpResponses"))
            {
                this.context.Add("HttpResponses", new List<int>());
            }

            try
            {
                var response = (HttpWebResponse)request.GetResponse();

                this.context.Get<List<int>>("HttpResponses").Add((int)response.StatusCode);
                this.context.Set(response);

                using Stream responseStream = response.GetResponseStream();
                using var responseReader = new StreamReader(responseStream);
                string responseBody = responseReader.ReadToEnd();
                this.context.Set(responseBody, "ResponseBody");
            }
            catch (WebException ex)
            {
                if (ex.Response is HttpWebResponse response)
                {
                    this.context.Get<List<int>>("HttpResponses").Add((int)response.StatusCode);
                }
            }
        }

        [When("I post the object called '(.*)' to the workflow engine path '(.*)'")]
        public void WhenIPostTheObjectCalledToTheWorkflowEnginePath(string instanceName, string path)
        {
            string url = WorkflowFunctionBindings.EngineHostBaseUrl + path;
            this.PostContextObjectToEndpoint(instanceName, url);
        }

        [When("I post the object called '(.*)' to the workflow message processing path '(.*)'")]
        public void WhenIPostTheObjectCalledToTheWorkflowMessageProcessingPath(string instanceName, string path)
        {
            string url = WorkflowFunctionBindings.MessageProcessingHostBaseUrl + path;
            this.PostContextObjectToEndpoint(instanceName, url);
        }

        [Given("I have posted the workflow called '(.*)' to the workflow engine path '(.*)'")]
        [When("I post the workflow called '(.*)' to the workflow engine path '(.*)'")]
        public void WhenIPostTheWorkflowCalledToTheWorkflowEnginePath(string workflowName, string path)
        {
            string url = WorkflowFunctionBindings.EngineHostBaseUrl + path;
            url = url.Replace("{tenantId}", this.transientTenantManager.PrimaryTransientClient.Id);
            Workflow workflow = this.context.Get<Workflow>(workflowName);

            // When POSTing, we shouldn't send an etag.
            workflow.ETag = null;
            this.SendObjectToEndpoint(workflow, url);
        }

        [When("I put the workflow called '(.*)' to the workflow engine path '(.*)'")]
        public void WhenIPutTheWorkflowCalledToTheWorkflowEnginePath(string workflowName, string path)
        {
            string url = WorkflowFunctionBindings.EngineHostBaseUrl + path;
            url = url.Replace("{tenantId}", this.transientTenantManager.PrimaryTransientClient.Id);
            Workflow workflow = this.context.Get<Workflow>(workflowName);
            this.SendObjectToEndpoint(workflow, url, "PUT");
        }

        [When("I put the workflow called '(.*)' to the workflow engine path '(.*)' with an If-Match header value from the etag of the workflow")]
        public void WhenIPutTheWorkflowCalledToTheWorkflowEnginePathWithAnIfMatchHeaderValueFromTheEtagOfTheWorkflow(string workflowName, string path)
        {
            string url = WorkflowFunctionBindings.EngineHostBaseUrl + path;
            url = url.Replace("{tenantId}", this.transientTenantManager.PrimaryTransientClient.Id);
            Workflow workflow = this.context.Get<Workflow>(workflowName);
            var headers = new Dictionary<HttpRequestHeader, string>
            {
                { HttpRequestHeader.IfMatch, workflow.ETag },
            };

            this.SendObjectToEndpoint(workflow, url, "PUT", headers);
        }

        [When("I put the workflow called '(.*)' to the workflow engine path '(.*)' with '(.*)' as the If-Match header value")]
        public void WhenIPutTheWorkflowCalledToTheWorkflowEnginePathWithAnIfMatchHeaderValueOf(string workflowName, string path, string ifMatchHeaderValue)
        {
            string url = WorkflowFunctionBindings.EngineHostBaseUrl + path;
            url = url.Replace("{tenantId}", this.transientTenantManager.PrimaryTransientClient.Id);
            Workflow workflow = this.context.Get<Workflow>(workflowName);
            var headers = new Dictionary<HttpRequestHeader, string>
            {
                { HttpRequestHeader.IfMatch, ifMatchHeaderValue },
            };

            this.SendObjectToEndpoint(workflow, url, "PUT", headers);
        }

        private void PostContextObjectToEndpoint(string instanceName, string url)
        {
            url = url.Replace("{tenantId}", this.transientTenantManager.PrimaryTransientClient.Id);

            foreach (object obj in this.context.Get<IEnumerable<object>>(instanceName))
            {
                this.SendObjectToEndpoint(obj, url);
            }
        }

        private void SendObjectToEndpoint(object instance, string url, string method = "POST", Dictionary<HttpRequestHeader, string> headers = null)
        {
            IJsonSerializerSettingsProvider serializationSettingsProvider = ContainerBindings.GetServiceProvider(this.featureContext).GetRequiredService<IJsonSerializerSettingsProvider>();
            string body = JsonConvert.SerializeObject(instance, serializationSettingsProvider.Instance);
            var address = new Uri(url);

            HttpWebRequest request = WebRequest.CreateHttp(address);
            request.ContentType = "application/json";
            request.Method = method;

            if (headers != null)
            {
                foreach (KeyValuePair<HttpRequestHeader, string> header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            var requestWriter = new StreamWriter(request.GetRequestStream());
            requestWriter.Write(body);
            requestWriter.Close();

            if (!this.context.ContainsKey("HttpResponses"))
            {
                this.context.Add("HttpResponses", new List<int>());
            }

            try
            {
                var response = (HttpWebResponse)request.GetResponse();

                this.context.Get<List<int>>("HttpResponses").Add((int)response.StatusCode);
            }
            catch (WebException ex)
            {
                if (ex.Response is HttpWebResponse response)
                {
                    this.context.Get<List<int>>("HttpResponses").Add((int)response.StatusCode);
                }
            }
        }
    }
}