// See https://aka.ms/new-console-template for more information
using Marain.Workflows;

#nullable disable annotations
public class ApprovalCondition : IWorkflowCondition
{
    public string ContentType => throw new NotImplementedException();

    public string Id { get; set; } = Guid.NewGuid().ToString();

    public Task<bool> EvaluateAsync(WorkflowInstance instance, IWorkflowTrigger trigger)
    {

        Console.Write(" Would you like to approve this expense claim? reply \"Yes\" or \"No\" and press <Enter>...");
        string input = Console.ReadLine();
        if (input == "Yes") { return Task.FromResult(true); } else { return Task.FromResult(false); }
    }

    // Any instance in a state with a transition that has this condition in its list of conditions will process
    // a trigger containing a subject of "Approval"
    public IEnumerable<string> GetInterests(WorkflowInstance instance)
    {
        return new[]
        {
            "Approval"
        };
    }
}


