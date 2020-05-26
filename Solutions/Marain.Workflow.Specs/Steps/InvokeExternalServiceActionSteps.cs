// <copyright file="InvokeExternalServiceActionSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Specs.Steps
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Corvus.Extensions.Json;
    using Corvus.Identity.ManagedServiceIdentity.ClientAuthentication;
    using Corvus.Tenancy;
    using Corvus.Testing.SpecFlow;
    using Marain.Workflows.Specs.Bindings;
    using Marain.Workflows.Specs.TestObjects;
    using Marain.Workflows.Specs.TestObjects.Triggers;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using NUnit.Framework;
    using TechTalk.SpecFlow;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented

    [Binding]
    public class InvokeExternalServiceActionSteps
    {
        private readonly ScenarioContext scenarioContext;
        private readonly FeatureContext featureContext;
        private InvokeExternalServiceAction action;
        private ExternalServiceBindings.ExternalService.RequestInfo requestInfo;

        public InvokeExternalServiceActionSteps(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            this.scenarioContext = scenarioContext;
            this.featureContext = featureContext;
        }

        [Given("I have created and persisted a workflow containing an external action with id '(.*)'")]
        public async Task GivenGivenIHaveCreatedAndPersistedAWorkflowContainingAnExternalActionWithIdAsync(string workflowId)
        {
            ExternalServiceBindings.ExternalService externalService = ExternalServiceBindings.GetService(this.scenarioContext);

            IServiceProvider container = ContainerBindings.GetServiceProvider(this.featureContext);

            IServiceIdentityTokenSource serviceIdentityTokenSource =
                container.GetRequiredService<IServiceIdentityTokenSource>();

            IJsonSerializerSettingsProvider serializerSettingsProvider =
                container.GetRequiredService<IJsonSerializerSettingsProvider>();

            ILogger<InvokeExternalServiceAction> logger = container.GetRequiredService<ILogger<InvokeExternalServiceAction>>();

            Workflow workflow = ExternalActionWorkflowFactory.Create(
                workflowId,
                externalService.TestUrl.ToString(),
                serviceIdentityTokenSource,
                serializerSettingsProvider,
                logger);

            this.action = workflow.GetInitialState().Transitions.Single().Actions.OfType<InvokeExternalServiceAction>().Single();

            ITenantedWorkflowStoreFactory storeFactory = container.GetRequiredService<ITenantedWorkflowStoreFactory>();

            ITenantProvider tenantProvider = container.GetRequiredService<ITenantProvider>();

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

        [When("I send a trigger that will execute the action with a trigger id of '(.*)'")]
        public async Task WhenISendATriggerThatWillExecuteTheAction(string triggerId)
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

        [Then("the action endpoint should have been invoked")]
        public void ThenTheActionEndpointShouldHaveBeenInvoked()
        {
            ExternalServiceBindings.ExternalService externalService = ExternalServiceBindings.GetService(this.scenarioContext);
            Assert.AreEqual(1, externalService.Requests.Count);
            this.requestInfo = externalService.Requests.Single();
            Assert.AreEqual(this.action.ExternalUrl, this.requestInfo.Url.ToString());
            Assert.AreEqual("POST", this.requestInfo.Verb);

            IJsonSerializerSettingsProvider serializationSettingsProvider =
                ContainerBindings.GetServiceProvider(this.featureContext).GetRequiredService<IJsonSerializerSettingsProvider>();

            CommonSteps.RequestBody = JsonConvert.DeserializeObject<ExternalServiceWorkflowRequest>(
                this.requestInfo.RequestBody,
                serializationSettingsProvider.Instance);
        }

        [Then("the Authorization header should be of type Bearer, using a token representing the managed service identity with the resource specified by the action")]
        public async Task ThenTheAuthorizationHeaderShouldBeOfTypeBearerUsingATokenRepresentingTheManagedServiceIdentityAsync()
        {
            Assert.IsTrue(
                this.requestInfo.Headers.TryGetValue("Authorization", out string authorizationHeader),
                "Should contain authorization header");

            IServiceIdentityTokenSource serviceIdentityTokenSource =
                ContainerBindings.GetServiceProvider(this.featureContext).GetRequiredService<IServiceIdentityTokenSource>();

            string token =
                await serviceIdentityTokenSource.GetAccessToken(this.action.MsiAuthenticationResource).ConfigureAwait(false);

            string expectedHeader = "Bearer " + token;

            Assert.AreEqual(expectedHeader, authorizationHeader);
        }

        [Then("the request body InvokedById should match the action id")]
        public void ThenTheResponseBodyInvokedByIdShouldMatchTheActionId()
        {
            Assert.AreEqual(this.action.Id, CommonSteps.RequestBody.InvokedById);
        }
    }
}
