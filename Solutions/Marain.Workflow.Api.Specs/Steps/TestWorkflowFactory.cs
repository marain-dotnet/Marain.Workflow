// <copyright file="TestWorkflowFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

// Is it OK to have nullability enabled in here?
#nullable enable

namespace Marain.Workflows.Api.Specs.Steps
{
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public static class TestWorkflowFactory
    {
        public static Workflow? Get(string name, string id)
        {
            return Get(name, id, null);
        }

        public static Workflow? Get(string name, string id, IReadOnlyList<WorkflowEventSubscription>? workflowEventSubscriptions)
        {
            return name switch
            {
                "SimpleExpensesWorkflow" => GetSimpleExpensesWorkflow(id, workflowEventSubscriptions),
                _ => null,
            };
        }

        public static Workflow GetSimpleExpensesWorkflow(string id, IReadOnlyList<WorkflowEventSubscription>? workflowEventSubscriptions)
        {
            Dictionary<string, WorkflowState> states = new();
            WorkflowState waitingForApproval = states.AddState("waiting-for-approval", "Waiting for approval");
            WorkflowState waitingForPayment = states.AddState("waiting-for-payment", "Waiting for payment");
            WorkflowState paid = states.AddState("paid", "Paid");
            WorkflowState abandoned = states.AddState("abandoned", "Abandoned");
            WorkflowState waitingToBeSubmitted = states.AddState("waiting-for-submission", "Waiting to be submitted");

            WorkflowTransition updateTransition = waitingToBeSubmitted.CreateTransition(waitingToBeSubmitted, "update", "Update");
            updateTransition.Conditions.Add(new HostedWorkflowTriggerNameCondition { TriggerName = "Update" });

            WorkflowTransition submitTransition = waitingToBeSubmitted.CreateTransition(waitingForApproval, "submit", "Submit");
            submitTransition.Conditions.Add(new HostedWorkflowTriggerNameCondition { TriggerName = "Submit" });

            WorkflowTransition abandonTransition = waitingToBeSubmitted.CreateTransition(abandoned, "abandon", "Abandon");
            abandonTransition.Conditions.Add(new HostedWorkflowTriggerNameCondition { TriggerName = "Abandon" });

            WorkflowTransition approveTransition = waitingForApproval.CreateTransition(waitingForPayment, "approve", "Approve");
            approveTransition.Conditions.Add(new HostedWorkflowTriggerNameCondition { TriggerName = "Approve" });

            WorkflowTransition rejectTransition = waitingForApproval.CreateTransition(waitingToBeSubmitted, "reject", "Reject");
            rejectTransition.Conditions.Add(new HostedWorkflowTriggerNameCondition { TriggerName = "Reject" });

            WorkflowTransition payTransition = waitingForPayment.CreateTransition(paid, "pay", "Pay");
            payTransition.Conditions.Add(new HostedWorkflowTriggerNameCondition { TriggerName = "Pay" });

            var workflow = new Workflow(
                id: id,
                states: states,
                initialStateId: waitingToBeSubmitted.Id,
                displayName: "Simple expenses workflow",
                workflowEventSubscriptions: workflowEventSubscriptions);

            return workflow;
        }
    }
}