// <copyright file="CommonSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Specs.Steps
{
    using System.Linq;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Corvus.SpecFlow.Extensions;
    using Corvus.Tenancy;
    using Marain.Workflows.Specs.Bindings;
    using Marain.Workflows.Specs.TestObjects;
    using Marain.Workflows.Specs.TestObjects.Triggers;
    using Microsoft.Extensions.DependencyInjection;
    using NUnit.Framework;
    using TechTalk.SpecFlow;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented

    [Binding]
    public class CommonSteps
    {
        private readonly ScenarioContext scenarioContext;
        private readonly FeatureContext featureContext;

        public CommonSteps(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            this.scenarioContext = scenarioContext;
            this.featureContext = featureContext;
        }

        public static ExternalServiceWorkflowRequest RequestBody { get; set; }

        public static CauseExternalInvocationTrigger TriggerIn { get; set; }

        [When("I create and persist a new instance with Id '(.*)' of the workflow with Id '(.*)' and supply the following context items")]
        [Given("I have created and persisted a new instance with Id '(.*)' of the workflow with Id '(.*)' and supplied the following context items")]
        public async Task WhenICreateANewInstanceCalledOfTheWorkflowWithId(
            string instanceId,
            string workflowId,
            Table table)
        {
            var context = table.Rows.ToDictionary(x => x["Key"], x => x["Value"]);

            IWorkflowEngineFactory engineFactory = ContainerBindings.GetServiceProvider(this.featureContext).GetService<IWorkflowEngineFactory>();
            ITenantProvider tenantProvider = ContainerBindings.GetServiceProvider(this.featureContext).GetService<ITenantProvider>();
            IWorkflowEngine engine = await engineFactory.GetWorkflowEngineAsync(tenantProvider.Root).ConfigureAwait(false);

            await engine.StartWorkflowInstanceAsync(new StartWorkflowInstanceRequest { Context = context, WorkflowId = workflowId, WorkflowInstanceId = instanceId }).ConfigureAwait(false);
        }

        [Given("the external service will return a (.*) status")]
        public void GivenITheExternalServiceWillReturnAStatus(int statusCode)
        {
            ExternalServiceBindings.GetService(this.scenarioContext).StatusCode = statusCode;
        }

        [Given("the workflow trigger queue is ready to process new triggers")]
        public void GivenTheWorkflowTriggerQueueIsReadyToProcessNewTriggers()
        {
            var queue = (InMemoryWorkflowMessageQueue)ContainerBindings.GetServiceProvider(this.featureContext).GetService<IWorkflowMessageQueue>();
            queue.StartProcessing();
        }

        [When("I wait for all triggers to be processed")]
        public async Task WhenIWaitForAllTriggersToBeProcessed()
        {
            var queue = (InMemoryWorkflowMessageQueue)ContainerBindings.GetServiceProvider(this.featureContext).GetService<IWorkflowMessageQueue>();
            await queue.FinishProcessing().ConfigureAwait(false);
        }

        [Then("the Content-Type header should be '(.*)'")]
        public void ThenTheContent_TypeHeaderShouldBe(string contentType)
        {
            ExternalServiceBindings.ExternalService.RequestInfo requestInfo = ExternalServiceBindings.GetService(this.scenarioContext).Requests.Single();
            Assert.IsTrue(requestInfo.Headers.TryGetValue("Content-Type", out string contentTypeHeader), "Should contain Content-Type header");
            var parsedHeader = MediaTypeHeaderValue.Parse(contentTypeHeader);
            Assert.AreEqual(contentType, parsedHeader.MediaType);
        }

        [Then("the request body WorkflowId should be '(.*)'")]
        public void ThenTheResponseBodyWorkflowIdShouldBe(string workflowId)
        {
            Assert.AreEqual(workflowId, RequestBody.WorkflowId);
        }

        [Then("the request body WorkflowInstanceId should be '(.*)'")]
        public void ThenTheResponseBodyWorkflowInstanceIdShouldBe(string workflowInstanceId)
        {
            Assert.AreEqual(workflowInstanceId, RequestBody.WorkflowInstanceId);
        }

        [Then("the request body Trigger matches the input trigger")]
        public void ThenTheResponseBodyTriggerMatchesTheInputTrigger()
        {
            var triggerOut = (CauseExternalInvocationTrigger)RequestBody.Trigger;
            Assert.AreEqual(TriggerIn.Id, triggerOut.Id, "Id");
            Assert.AreEqual(TriggerIn.ContentType, triggerOut.ContentType, "ContentType");
            Assert.AreEqual(TriggerIn.PartitionKey, triggerOut.PartitionKey, "PartitionKey");
            Assert.AreEqual(TriggerIn.OtherData, triggerOut.OtherData, "OtherData");
            Assert.AreEqual(TriggerIn.GetSubjects(), triggerOut.GetSubjects(), "Subjects");
        }

        [Then("the request body ContextProperties key '(.*)' has the value '(.*)'")]
        public void ThenTheResponseBodyContextPropertiesKeyHasTheValue(string key, string value)
        {
            Assert.AreEqual(value, RequestBody.ContextProperties[key]);
        }

        [Then("the request body ContextProperties has (.*) values")]
        public void ThenTheResponseBodyContextPropertiesHasValues(int count)
        {
            Assert.AreEqual(count, RequestBody.ContextProperties.Count);
        }
    }
}
