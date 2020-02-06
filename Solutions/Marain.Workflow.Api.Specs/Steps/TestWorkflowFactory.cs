// <copyright file="TestWorkflowFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.Specs.Steps
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable SA1600 // Elements should be documented

    public static class TestWorkflowFactory
    {
        public static Workflow Get(string name)
        {
            return name switch
            {
                "SimpleExpensesWorkflow" => GetSimpleExpensesWorkflow(),
                _ => null,
            };
        }

        public static Workflow GetSimpleExpensesWorkflow()
        {
            var workflow = new Workflow { DisplayName = "Simple expenses workflow" };

            WorkflowState waitingToBeSubmitted = workflow.CreateState(displayName: "Waiting to be submitted");
            WorkflowState waitingForApproval = workflow.CreateState(displayName: "Waiting for approval");
            WorkflowState waitingForPayment = workflow.CreateState(displayName: "Waiting for payment");
            WorkflowState paid = workflow.CreateState(displayName: "Paid");
            WorkflowState abandoned = workflow.CreateState(displayName: "Abandoned");

            workflow.SetInitialState(waitingToBeSubmitted);

            WorkflowTransition updateTransition = waitingToBeSubmitted.CreateTransition(waitingToBeSubmitted, displayName: "Update");
            updateTransition.Conditions.Add(new HostedWorkflowTriggerNameCondition { TriggerName = "Update" });

            WorkflowTransition submitTransition = waitingToBeSubmitted.CreateTransition(waitingForApproval, displayName: "Submit");
            submitTransition.Conditions.Add(new HostedWorkflowTriggerNameCondition { TriggerName = "Submit" });

            WorkflowTransition abandonTransition = waitingToBeSubmitted.CreateTransition(abandoned, displayName: "Abandon");
            abandonTransition.Conditions.Add(new HostedWorkflowTriggerNameCondition { TriggerName = "Abandon" });

            WorkflowTransition approveTransition = waitingForApproval.CreateTransition(waitingForPayment, displayName: "Approve");
            approveTransition.Conditions.Add(new HostedWorkflowTriggerNameCondition { TriggerName = "Approve" });

            WorkflowTransition rejectTransition = waitingForApproval.CreateTransition(waitingToBeSubmitted, displayName: "Reject");
            rejectTransition.Conditions.Add(new HostedWorkflowTriggerNameCondition { TriggerName = "Reject" });

            WorkflowTransition payTransition = waitingForPayment.CreateTransition(paid, displayName: "Pay");
            payTransition.Conditions.Add(new HostedWorkflowTriggerNameCondition { TriggerName = "Pay" });

            return workflow;
        }
    }
}
