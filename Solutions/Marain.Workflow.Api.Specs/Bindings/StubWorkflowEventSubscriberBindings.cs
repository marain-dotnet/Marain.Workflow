// <copyright file="StubWorkflowEventSubscriberBindings.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.Specs.Bindings
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Corvus.Testing.ReqnRoll;
    using Marain.Workflows.Api.Specs.Helpers;
    using Reqnroll;

    [Binding]
    public static class StubWorkflowEventSubscriberBindings
    {
        [AfterScenario]
        public static Task CleanUpStubWorkflowEventSubscribers(ScenarioContext scenarioContext)
        {
            IEnumerable<StubWorkflowEventSubscriber> subscribers =
                scenarioContext.Where(x => x.Value is StubWorkflowEventSubscriber).Select(x => (StubWorkflowEventSubscriber)x.Value);

            return Task.WhenAll(subscribers.Select(s => scenarioContext.RunAndStoreExceptionsAsync(() => s.DisposeAsync().AsTask())));
        }
    }
}