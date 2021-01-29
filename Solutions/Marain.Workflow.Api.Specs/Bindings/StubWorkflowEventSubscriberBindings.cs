namespace Marain.Workflows.Api.Specs.Bindings
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Corvus.Testing.SpecFlow;
    using Marain.Workflows.Api.Specs.Helpers;
    using TechTalk.SpecFlow;

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
