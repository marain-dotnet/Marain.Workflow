// <copyright file="WorkflowInstanceStoreBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Specs.Bindings
{
    using Corvus.Testing.SpecFlow;
    using Microsoft.Extensions.DependencyInjection;
    using TechTalk.SpecFlow;

    [Binding]
    public static class WorkflowInstanceStoreBindings
    {
        [BeforeScenario("usingInMemoryNEventStore", Order = ContainerBeforeScenarioOrder.PopulateServiceCollection)]
        public static void RegisterInMemoryWorkflowInstanceStore(ScenarioContext scenarioContext)
        {
            ContainerBindings.ConfigureServices(
                scenarioContext,
                services => services.AddInMemoryWorkflowInstanceEventStore());
        }
    }
}
