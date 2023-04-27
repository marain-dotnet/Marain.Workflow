using Marain.Workflows;
using Corvus.Leasing.Internal;
using System.Runtime.CompilerServices;
using Marain.Workflows.CloudEvents;
using Microsoft.Extensions.Logging.Abstractions;

public class WorkflowEngineBuilder
{
    public SimpleWorkflowInstanceStore WorkflowInstanceStore { get; private set; }

    public WorkflowEngine BuildWorkflowEngine(IWorkflowStore workflowStore)
    {
        // Create in-memory workflow instance store
        this.WorkflowInstanceStore = new SimpleWorkflowInstanceStore();
        // Create an in-memory ILeaseProvider
        InMemoryLeaseProvider inMemoryLeaseProvider = new();
        // Crate a Cloud event publisher - call it ConsoleCloudEventPublisher
        ConsoleCloudEventPublisher consoleCloudEventPublisher = new();
        // Create an a null logger - there's a C# class for this
        NullLogger<IWorkflowEngine> nullLogger = new NullLogger<IWorkflowEngine>();
        // Create and return a workflow engine using all of the above pieces

        // Create workflow engine
        WorkflowEngine workflowEngine = new WorkflowEngine(
            workflowStore,
            this.WorkflowInstanceStore,
            inMemoryLeaseProvider,
            "simpleWorkflowExample",
            consoleCloudEventPublisher, //ConsoleCloudEventPublisher, NullCloudEventPublisher
            nullLogger);

        return workflowEngine;
    }
}