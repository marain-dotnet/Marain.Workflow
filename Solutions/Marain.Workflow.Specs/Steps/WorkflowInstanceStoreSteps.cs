﻿// <copyright file="WorkflowInstanceStoreSteps.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Specs.Steps
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Corvus.Json;
    using Corvus.Tenancy;
    using Corvus.Testing.SpecFlow;
    using Microsoft.Extensions.DependencyInjection;
    using TechTalk.SpecFlow;

    [Binding]
    public class WorkflowInstanceStoreSteps : Steps
    {
        [Given("I have stored the workflow instance called '(.*)'")]
        [When("I store the workflow instance called '(.*)'")]
        public async Task WhenIStoreTheWorkflowInstanceWithId(string instanceId)
        {
            IPropertyBagFactory propertyBagFactory = ContainerBindings.GetServiceProvider(this.ScenarioContext)
                .GetRequiredService<IPropertyBagFactory>();

            var tenant = new Tenant("test", "test", propertyBagFactory.Create(Enumerable.Empty<KeyValuePair<string, object>>())) as ITenant;
            this.ScenarioContext.Set(tenant);

            IWorkflowInstanceStore store = await this.GetWorkflowInstanceStore(tenant).ConfigureAwait(false);

            WorkflowInstance instance = this.ScenarioContext.Get<WorkflowInstance>(instanceId);

            try
            {
                await store.UpsertWorkflowInstanceAsync(instance).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                this.ScenarioContext.Set(ex);
            }
        }

        [Given("I have loaded the workflow instance with Id '(.*)' and called it '(.*)'")]
        [When("I load the workflow instance with Id '(.*)' and call it '(.*)'")]
        public async Task WhenILoadTheWorkflowInstanceWithId(string instanceId, string instanceName)
        {
            ITenant tenant = this.ScenarioContext.Get<ITenant>();
            IWorkflowInstanceStore store = await this.GetWorkflowInstanceStore(tenant).ConfigureAwait(false);
            WorkflowInstance instance = await store.GetWorkflowInstanceAsync(instanceId).ConfigureAwait(false);
            this.ScenarioContext.Set(instance, instanceName);
        }

        private Task<IWorkflowInstanceStore> GetWorkflowInstanceStore(ITenant tenant)
        {
            IServiceProvider serviceProvider = ContainerBindings.GetServiceProvider(this.ScenarioContext);

            ITenantedWorkflowInstanceStoreFactory workflowInstanceStoreFactory = serviceProvider
                .GetRequiredService<ITenantedWorkflowInstanceStoreFactory>();

            return workflowInstanceStoreFactory.GetWorkflowInstanceStoreForTenantAsync(tenant);
        }
    }
}
