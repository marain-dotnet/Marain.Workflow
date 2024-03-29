﻿// <copyright file="TestWorkflowFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Api.Specs.Steps
{
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

            WorkflowState waitingToBeSubmitted = workflow.CreateState("waiting-for-submission", "Waiting to be submitted");
            WorkflowState waitingForApproval = workflow.CreateState("waiting-for-approval", "Waiting for approval");
            WorkflowState waitingForPayment = workflow.CreateState("waiting-for-payment", "Waiting for payment");
            WorkflowState paid = workflow.CreateState("paid", "Paid");
            WorkflowState abandoned = workflow.CreateState("abandoned", "Abandoned");

            workflow.SetInitialState(waitingToBeSubmitted);

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

            return workflow;
        }
    }
}