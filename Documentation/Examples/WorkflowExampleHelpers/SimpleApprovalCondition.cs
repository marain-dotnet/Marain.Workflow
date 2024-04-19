// See https://aka.ms/new-console-template for more information
using Marain.Workflows;
using System;

#nullable disable annotations
public class SimpleApprovalCondition : IWorkflowCondition
{
    public string ContentType => throw new NotImplementedException();

    public string Id { get; set; } = Guid.NewGuid().ToString();

    public bool IsExpenseClaimApproved { get; private set; } = false;

    public void ApproveExpenseClaim()
    {
        if (IsExpenseClaimApproved == true)
        {
            throw new Exception("Expense claim is already approved");
        }

        this.IsExpenseClaimApproved = true;
    }

    public Task<bool> EvaluateAsync(WorkflowInstance instance, IWorkflowTrigger trigger)
    {
        return Task.FromResult(this.IsExpenseClaimApproved);
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


