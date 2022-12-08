using Marain.Workflows;
using Corvus.Leasing.Internal;
using System.Runtime.CompilerServices;
using Marain.Workflows.CloudEvents;
using Microsoft.Extensions.Logging;

public static class WorkflowEngineBuilder
{
    public static BuildWorkflowEngine()
    {
        // Create in-memory workflow store
        SimpleWorkflowStore workflowStore = new SimpleWorkflowStore();
        // Create in-memory workflow instance store
        SimpleWorkflowInstanceStore workflowInstanceStore = new SimpleWorkflowInstanceStore();
        // Create an in-memory ILeaseProvider
        InMemoryLeaseProvider inMemoryLeaseProvider = new();
        // Crate a Cloud event publisher - call it ConsoleCloudEventPublisher
        SomethingCloudEventPublisher dummyCloudEventPublisher = new();
        // Create an a null logger - there's a C# class for this

        // Create and return a workflow engine using all of the above pieces

    }
}