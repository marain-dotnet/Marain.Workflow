using Marain.Workflows;

#nullable disable annotations
public class SimpleWorkflowInstanceStore : IWorkflowInstanceStore
{
    private readonly List<WorkflowInstance> workflowInstances = new();

    public Task DeleteWorkflowInstanceAsync(string workflowInstanceId, string partitionKey = null)
    {
        throw new NotImplementedException();
    }

    // TODO : Change use of Task.Factory to Task.GetResult
    public Task<int> GetMatchingWorkflowInstanceCountForSubjectsAsync(IEnumerable<string> subjects)
    {
        return Task.FromResult(GetMatchingWorkflowInstancesForSubjects(subjects).Count());
    }

    // TODO : Change use of Task.Factory to Task.GetResult
    public Task<IEnumerable<string>> GetMatchingWorkflowInstanceIdsForSubjectsAsync(IEnumerable<string> subjects, int pageSize, int pageNumber)
    {
        return Task.FromResult(GetMatchingWorkflowInstancesForSubjects(subjects).Select(wi => wi.Id));
    }

    public Task<WorkflowInstance> GetWorkflowInstanceAsync(string workflowInstanceId, string partitionKey = null)
    {
        return Task.FromResult(workflowInstances.Single(wi => wi.Id == workflowInstanceId));
    }
        
    public Task UpsertWorkflowInstanceAsync(WorkflowInstance workflowInstance, string partitionKey = null)
    {
        int existingIndex = workflowInstances.FindIndex(wi => wi.Id == workflowInstance.Id);
        if (existingIndex >= 0)
        {
            workflowInstances[existingIndex] = workflowInstance;
        }
        else
        {
            workflowInstances.Add(workflowInstance); 
        }
        return Task.CompletedTask;
    }

    private IEnumerable<WorkflowInstance> GetMatchingWorkflowInstancesForSubjects(IEnumerable<string> subjects)
    {
        return workflowInstances.Where(wi => wi.Interests.Intersect(subjects).Count() > 0);
    }
}


