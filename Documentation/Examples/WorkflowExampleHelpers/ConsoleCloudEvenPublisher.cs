using Marain.Workflows;
using Marain.Workflows.CloudEvents;

#nullable disable annotations

// An ICloudEventPublisher that writes event information to the console
internal class ConsoleCloudEventPublisher : ICloudEventDataPublisher
{
    public Task PublishWorkflowEventDataAsync<T>(string source, string workflowEventType, string subject, string dataContentType, T eventData, IEnumerable<WorkflowEventSubscription> subscriptions)
    {
        Console.WriteLine($"cloud event: {source}, workflowEventType: {workflowEventType}, subject: {subject}, dataContentType: {dataContentType},  eventData: {eventData},  subscriptions: {subscriptions}");
        return Task.CompletedTask;
    }
}