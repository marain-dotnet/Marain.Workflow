using Marain.Workflows;

#nullable disable annotations
public class SimpleWorkflowStore : IWorkflowStore
{
    private readonly List<Workflow> workflows = new();

    // TODO : Change use of Task.Factory to Task.GetResult
    public Task<Workflow> GetWorkflowAsync(string workflowId, string partitionKey = null)
    {

        return Task.FromResult(workflows.Where(w => w.Id == workflowId).First());
    }

    // TODO : Change use of Task.Factory to Task.GetResult
    public Task UpsertWorkflowAsync(Workflow workflow, string partitionKey = null)
    {
        int existingIndex = workflows.FindIndex(w => w.Id == workflow.Id);
        if (existingIndex >= 0)
        {
            workflows[existingIndex] = workflow;
        }
        else
        {
            workflows.Add(workflow);
        }
        return Task.CompletedTask;
    }
}


