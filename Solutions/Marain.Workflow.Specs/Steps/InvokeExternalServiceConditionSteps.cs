// <copyright file="InvokeExternalServiceConditionSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Specs.Steps
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Corvus.Extensions.Json;
    using Corvus.Identity.ClientAuthentication;
    using Corvus.Tenancy;
    using Corvus.Testing.SpecFlow;
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
        private readonly ScenarioContext scenarioContext;
        private readonly FeatureContext featureContext;
        private InvokeExternalServiceCondition condition;
        private ExternalServiceBindings.ExternalService.RequestInfo requestInfo;

        public InvokeExternalServiceConditionSteps(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            this.scenarioContext = scenarioContext;
            this.featureContext = featureContext;
        }

        [Given("I have created and persisted a workflow containing an external condition with id '(.*)'")]
        public async Task GivenGivenIHaveCreatedAndPersistedAWorkflowContainingAnExternalConditionWithIdAsync(string workflowId)
        {
            ExternalServiceBindings.ExternalService externalService = ExternalServiceBindings.GetService(this.scenarioContext);
            IServiceIdentityAccessTokenSource serviceIdentityTokenSource =
                ContainerBindings.GetServiceProvider(this.featureContext).GetRequiredService<IServiceIdentityAccessTokenSource>();

            IJsonSerializerSettingsProvider serializerSettingsProvider =
                ContainerBindings.GetServiceProvider(this.featureContext).GetRequiredService<IJsonSerializerSettingsProvider>();

            Workflow workflow = ExternalConditionWorkflowFactory.Create(
                workflowId,
                externalService.TestUrl.ToString(),
                serviceIdentityTokenSource,
                serializerSettingsProvider);

            this.condition = workflow.GetInitialState().Transitions
                                     .Single()
                                     .Conditions
                                     .OfType<InvokeExternalServiceCondition>()
                                     .Single();

            ITenantedWorkflowStoreFactory storeFactory = ContainerBindings.GetServiceProvider(this.featureContext)
                                                                    .GetService<ITenantedWorkflowStoreFactory>();

            ITenantProvider tenantProvider =
                ContainerBindings.GetServiceProvider(this.featureContext).GetRequiredService<ITenantProvider>();

            IWorkflowStore store = await storeFactory.GetWorkflowStoreForTenantAsync(tenantProvider.Root).ConfigureAwait(false);

            await WorkflowRetryHelper.ExecuteWithStandardTestRetryRulesAsync(async () =>
            {
                try
                {
                    Workflow oldWorkflow = await store.GetWorkflowAsync(workflowId).ConfigureAwait(false);
                    workflow.ETag = oldWorkflow.ETag;
                }
                catch (WorkflowNotFoundException)
                {
                    // Don't care if there is no old workflow.
                }

                await store.UpsertWorkflowAsync(workflow).ConfigureAwait(false);
            }).ConfigureAwait(false);
        }

        [Given("the external service response body will contain '(.*)'")]
        public void GivenTheExternalServiceResponseBodyWillContain(string content)
        {
            ExternalServiceBindings.ExternalService externalService = ExternalServiceBindings.GetService(this.scenarioContext);
            externalService.ConditionResponseBody = content;
        }

        [When("I send a trigger that will execute the condition with a trigger id of '(.*)'")]
        public async Task WhenISendATriggerThatWillExecuteTheConditionAsync(string triggerId)
        {
            CommonSteps.TriggerIn = new CauseExternalInvocationTrigger
            {
                Id = triggerId,
                OtherData = Guid.NewGuid().ToString(),
            };

            IWorkflowMessageQueue queue =
                ContainerBindings.GetServiceProvider(this.featureContext).GetService<IWorkflowMessageQueue>();

            await queue.EnqueueTriggerAsync(CommonSteps.TriggerIn, operationId: Guid.NewGuid()).ConfigureAwait(false);
        }

        [Then("the condition endpoint should have been invoked")]
        public void ThenTheConditionEndpointShouldHaveBeenInvoked()
        {
            ExternalServiceBindings.ExternalService externalService = ExternalServiceBindings.GetService(this.scenarioContext);
            Assert.AreEqual(1, externalService.Requests.Count);
            this.requestInfo = externalService.Requests.Single();
            Assert.AreEqual(this.condition.ExternalUrl, this.requestInfo.Url.ToString());
            Assert.AreEqual("POST", this.requestInfo.Verb);

            IJsonSerializerSettingsProvider serializationSettingsProvider =
                ContainerBindings.GetServiceProvider(this.featureContext).GetRequiredService<IJsonSerializerSettingsProvider>();

            CommonSteps.RequestBody = JsonConvert.DeserializeObject<ExternalServiceWorkflowRequest>(
                this.requestInfo.RequestBody,
                serializationSettingsProvider.Instance);
        }

        [Then("the Authorization header should be of type Bearer, using a token representing the managed service identity with the resource specified by the condition")]
        public async Task ThenTheAuthorizationHeaderShouldBeOfTypeBearerUsingATokenRepresentingTheManagedServiceIdentityWithTheResourceSpecifiedByTheConditionAsync()
        {
            Assert.IsTrue(
                this.requestInfo.Headers.TryGetValue("Authorization", out string authorizationHeader),
                "Should contain authorization header");

            IServiceIdentityAccessTokenSource tokenSource =
                ContainerBindings.GetServiceProvider(this.featureContext).GetRequiredService<IServiceIdentityAccessTokenSource>();

            AccessTokenDetail tokenDetail = await tokenSource.GetAccessTokenAsync(
                new AccessTokenRequest(new[] { $"{this.condition.MsiAuthenticationResource}/.default" }))
                .ConfigureAwait(false);
            string expectedHeader = "Bearer " + tokenDetail.AccessToken;
            Assert.AreEqual(expectedHeader, authorizationHeader);
        }

        [Then("the request body InvokedById should match the condition id")]
        public void ThenTheRequestBodyInvokedByIdShouldMatchTheConditionId()
        {
            Assert.AreEqual(this.condition.Id, CommonSteps.RequestBody.InvokedById);
        }
    }
}