// See https://aka.ms/new-console-template for more information
using Marain.Workflows;
using Microsoft.CodeAnalysis.CSharp.Syntax;
#nullable disable annotations
public class SimpleEntryCondition : IWorkflowCondition
{
    public string ContentType => throw new NotImplementedException();

    public string Id { get; set; } = Guid.NewGuid().ToString();

    public Task<bool> EvaluateAsync(WorkflowInstance instance, IWorkflowTrigger trigger)
    {
        bool dateIssued = instance.Context.TryGetValue("Date Issued", out string value);
        if (!dateIssued) { throw new Exception("Date issue context not available on this instance"); }
        if (!DateTime.TryParse(value, out DateTime result))
        {
            throw new Exception("Cannot parse string into DateTime");
        }

        // If the date issued was greater than two days ago return false and dot let the instance enter the
        // "Approved" state.
        if (DateTime.Now - result < TimeSpan.FromDays(2)) { return Task.FromResult(true); }
        else
        {
            Console.WriteLine("This expense claim was submitted more than two days ago and cannot be approved");
            return Task.FromResult(false);
        }
    }

    // Any instance in a state with a transition that has this condition in its list of conditions will process
    // a trigger containing a subject of "Approval"
    public IEnumerable<string> GetInterests(WorkflowInstance instance)
    {
        return new string[0];
    }
}


