// <copyright file="TestWorkflowFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Functions.Specs.Steps
{
    using Microsoft.Extensions.DependencyInjection;

    public static class TestWorkflowFactory
    {
        public static Workflow Get(string name)
        {
            switch (name)
            {
                case "SimpleExpensesWorkflow":
                    return GetSimpleExpensesWorkflow();
            }

            return null;
        }

        public static Workflow GetSimpleExpensesWorkflow()
        {
            var workflow = new Workflow { DisplayName = "Simple expenses workflow" };

            var waitingToBeSubmitted = workflow.CreateState(displayName: "Waiting to be submitted");
            var waitingForApproval = workflow.CreateState(displayName: "Waiting for approval");
            var waitingForPayment = workflow.CreateState(displayName: "Waiting for payment");
            var paid = workflow.CreateState(displayName: "Paid");
            var abandoned = workflow.CreateState(displayName: "Abandoned");

            workflow.SetInitialState(waitingToBeSubmitted);

            var updateTransition = waitingToBeSubmitted.CreateTransition(waitingToBeSubmitted, displayName: "Update");
            updateTransition.Conditions.Add(new HostedWorkflowTriggerNameCondition { TriggerName = "Update" });

            var submitTransition = waitingToBeSubmitted.CreateTransition(waitingForApproval, displayName: "Submit");
            submitTransition.Conditions.Add(new HostedWorkflowTriggerNameCondition { TriggerName = "Submit" });

            var abandonTransition = waitingToBeSubmitted.CreateTransition(abandoned, displayName: "Abandon");
            abandonTransition.Conditions.Add(new HostedWorkflowTriggerNameCondition { TriggerName = "Abandon" });

            var approveTransition = waitingForApproval.CreateTransition(waitingForPayment, displayName: "Approve");
            approveTransition.Conditions.Add(new HostedWorkflowTriggerNameCondition { TriggerName = "Approve" });

            var rejectTransition = waitingForApproval.CreateTransition(waitingToBeSubmitted, displayName: "Reject");
            rejectTransition.Conditions.Add(new HostedWorkflowTriggerNameCondition { TriggerName = "Reject" });

            var payTransition = waitingForPayment.CreateTransition(paid, displayName: "Pay");
            payTransition.Conditions.Add(new HostedWorkflowTriggerNameCondition { TriggerName = "Pay" });

            return workflow;
        }
    }
}
