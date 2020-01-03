// <copyright file="Steps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable CS1591 // Elements should be documented

namespace Marain.Workflows.Functions.Specs.Steps
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;

    using Marain.Serialization.Json;

    using Newtonsoft.Json;

    using NUnit.Framework;

    using TechTalk.SpecFlow;

    [Binding]
    public class WebRequestSteps
    {
        private readonly ScenarioContext context;

        public WebRequestSteps(ScenarioContext context)
        {
            this.context = context;
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

        [When(@"I post the object called ""(.*)"" to the endpoint ""(.*)""")]
        [When(@"I post the objects called ""(.*)"" to the endpoint ""(.*)""")]
        public void WhenIPostTheObjectCalledToTheEndpoint(string instanceName, string url)
        {
            foreach (object obj in this.context.Get<IEnumerable<object>>(instanceName))
            {
                this.PostObjectToEndpoint(obj, url);
            }
        }

        private void PostObjectToEndpoint(object instance, string url)
        {
            string body = JsonConvert.SerializeObject(instance, SerializerSettings.CreateSerializationSettings());
            var address = new Uri(url);

            HttpWebRequest request = WebRequest.CreateHttp(address);
            request.ContentType = "application/json";
            request.Method = "POST";
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

#pragma warning restore SA1600 // Elements should be documented
#pragma warning restore CS1591 // Elements should be documented