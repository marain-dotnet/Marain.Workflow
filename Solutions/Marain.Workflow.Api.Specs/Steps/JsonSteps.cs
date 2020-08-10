// <copyright file="JsonSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.Specs.Steps
{
    using System.Linq;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;
    using TechTalk.SpecFlow;

    [Binding]
    public class JsonSteps
    {
        private readonly ScenarioContext scenarioContext;

        public JsonSteps(ScenarioContext scenarioContext)
        {
            this.scenarioContext = scenarioContext;
        }

        [Then("the response object should have a property called '(.*)'")]
        public void ThenTheResponseObjectShouldHaveAPropertyCalled(string propertyPath)
        {
            this.GetRequiredTokenFromResponseObject(propertyPath);
        }

        [Then("the response object should have a string property called '(.*)' with value '(.*)'")]
        public void ThenTheResponseObjectShouldHaveAStringPropertyCalledWithValue(string propertyPath, string expectedValue)
        {
            JToken actualToken = this.GetRequiredTokenFromResponseObject(propertyPath);

            string actualValue = actualToken.Value<string>();
            Assert.AreEqual(expectedValue, actualValue);
        }

        [Then("the response object should not have a property called '(.*)'")]
        public void ThenTheResponseObjectShouldNotHaveAPropertyCalled(string propertyPath)
        {
            JObject data = this.scenarioContext.Get<JObject>();
            JToken token = data.SelectToken(propertyPath);
            Assert.IsNull(token);
        }

        [Then("the response object should have an array property called '(.*)' containing (.*) entries")]
        public void ThenTheResponseObjectShouldHaveAnArrayPropertyCalledContainingEntries(string propertyPath, int expectedEntryCount)
        {
            JToken actualToken = this.GetRequiredTokenFromResponseObject(propertyPath);
            JToken[] tokenArray = actualToken.ToArray();
            Assert.AreEqual(expectedEntryCount, tokenArray.Length);
        }

        [Given("I have stored the value of the response object property called '(.*)' as '(.*)'")]
        public void GivenIHaveStoredTheValueOfTheResponseObjectPropertyCalledAs(string propertyPath, string storeAsName)
        {
            JToken token = this.GetRequiredTokenFromResponseObject(propertyPath);
            string valueAsString = token.Value<string>();
            this.scenarioContext.Set(valueAsString, storeAsName);
        }

        private JToken GetRequiredTokenFromResponseObject(string propertyPath)
        {
            JObject data = this.scenarioContext.Get<JObject>();
            JToken token = data.SelectToken(propertyPath);
            Assert.IsNotNull(token);
            return token;
        }
    }
}
