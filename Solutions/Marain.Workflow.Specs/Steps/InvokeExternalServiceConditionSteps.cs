// <copyright file="CreateCatalogItemTrigger.cs" company="Endjin">
// Copyright (c) Endjin. All rights reserved.
// </copyright>

#pragma warning disable SA1600, CS1591

namespace Marain.Workflows.Specs.Steps
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Corvus.Extensions.Json;
    using Corvus.Identity.ManagedServiceIdentity.ClientAuthentication;
    using Corvus.SpecFlow.Extensions;
    using Corvus.Tenancy;
    using Marain.Workflows.Specs.Bindings;
    using Marain.Workflows.Specs.TestObjects;
    using Marain.Workflows.Specs.TestObjects.Triggers;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using TechTalk.SpecFlow;

    [Binding]
    public class InvokeExternalServiceConditionSteps
    {
        private InvokeExternalServiceCondition condition;
        private ExternalServiceBindings.ExternalService externalService;
        private ExternalServiceBindings.ExternalService.RequestInfo requestInfo;
        private readonly FeatureContext featureContext;
        private readonly ScenarioContext scenarioContext;

        public InvokeExternalServiceConditionSteps(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            this.featureContext = featureContext;
            this.scenarioContext = scenarioContext;
        }

        [Given("I have created and persisted a workflow containing an external condition with id '(.*)'")]
        public async Task GivenGivenIHaveCreatedAndPersistedAWorkflowContainingAnExternalConditionWithIdAsync(string workflowId)
        {
            this.externalService = ExternalServiceBindings.GetExternalService(this.scenarioContext);
            IServiceIdentityTokenSource tokenSource = ContainerBindings.GetServiceProvider(this.featureContext).GetRequiredService<IServiceIdentityTokenSource>();
            IJsonSerializerSettingsProvider serializerSettingsProvider = ContainerBindings.GetServiceProvider(this.featureContext).GetRequiredService<IJsonSerializerSettingsProvider>();
            Workflow workflow = ExternalConditionWorkflowFactory.Create(
                tokenSource,
                serializerSettingsProvider,
                workflowId,
                this.externalService.TestUrl.ToString());

            this.condition = workflow.GetInitialState().Transitions.Single().Conditions.OfType<InvokeExternalServiceCondition>().Single();

            IWorkflowEngineFactory engineFactory = ContainerBindings.GetServiceProvider(this.featureContext).GetRequiredService<IWorkflowEngineFactory>();
            ITenantProvider tenantProvider = ContainerBindings.GetServiceProvider(this.featureContext).GetRequiredService<ITenantProvider>();
            IWorkflowEngine engine = await engineFactory.GetWorkflowEngineAsync(tenantProvider.Root).ConfigureAwait(false);

            await engine.UpsertWorkflowAsync(workflow).ConfigureAwait(false);
        }

        [Given("the external service response body will contain '(.*)'")]
        public void GivenTheExternalServiceResponseBodyWillContain(string content)
        {
            this.externalService.ResponseBody = content;
        }

        [When("I send a trigger that will execute the condition with a trigger id of '(.*)'")]
        public async Task WhenISendATriggerThatWillExecuteTheConditionAsync(string triggerId)
        {
            CommonSteps.TriggerIn = new CauseExternalInvocationTrigger
            {
                Id = triggerId,
                OtherData = Guid.NewGuid().ToString()
            };

            IWorkflowMessageQueue queue = ContainerBindings.GetServiceProvider(this.featureContext).GetService<IWorkflowMessageQueue>();
            await queue.EnqueueTriggerAsync(CommonSteps.TriggerIn, operationId: Guid.NewGuid()).ConfigureAwait(false);
        }

        [Then("the condition endpoint should have been invoked")]
        public void ThenTheConditionEndpointShouldHaveBeenInvoked()
        {
            Assert.AreEqual(1, ExternalServiceBindings.GetExternalService(this.scenarioContext).Requests.Count);
            this.requestInfo = ExternalServiceBindings.GetExternalService(this.scenarioContext).Requests.Single();
            Assert.AreEqual(this.condition.ExternalUrl, this.requestInfo.Url.ToString());
            Assert.AreEqual("POST", this.requestInfo.Verb);

            IJsonSerializerSettingsProvider settingsProvider = ContainerBindings.GetServiceProvider(this.featureContext).GetRequiredService<IJsonSerializerSettingsProvider>();

            CommonSteps.RequestBody = JsonConvert.DeserializeObject<ExternalServiceWorkflowRequest>(
                this.requestInfo.RequestBody,
                settingsProvider.Instance);
        }

        [Then("the Authorization header should be of type Bearer, using a token representing the managed service identity with the resource specified by the condition")]
        public async Task ThenTheAuthorizationHeaderShouldBeOfTypeBearerUsingATokenRepresentingTheManagedServiceIdentityWithTheResourceSpecifiedByTheConditionAsync()
        {
            Assert.IsTrue(this.requestInfo.Headers.TryGetValue("Authorization", out string authorizationHeader), "Should contain authorization header");
            IServiceIdentityTokenSource tokenSource = ContainerBindings.GetServiceProvider(this.featureContext).GetRequiredService<IServiceIdentityTokenSource>();
            string token = await tokenSource.GetAccessToken(this.condition.MsiAuthenticationResource).ConfigureAwait(false);
            string expectedHeader = "Bearer " + token;
            Assert.AreEqual(expectedHeader, authorizationHeader);
        }

        [Then("the request body InvokedById should match the condition id")]
        public void ThenTheRequestBodyInvokedByIdShouldMatchTheConditionId()
        {
            Assert.AreEqual(this.condition.Id, CommonSteps.RequestBody.InvokedById);
        }
    }
}
