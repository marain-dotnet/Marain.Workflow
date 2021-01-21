// <copyright file="SimpleWorkflowFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Specs.TestObjects
{
    /// <summary>
    /// Factory for creating simple workflows for testing purposes.
    /// </summary>
    public static class SimpleWorkflowFactory
    {
        /// <summary>
        /// Creates a workflow with two states.
        /// </summary>
        /// <param name="id">The Id of the new workflow.</param>
        /// <returns>The new workflow.</returns>
        public static Workflow CreateTwoStateWorkflow(string id)
        {
            var result = new Workflow(id, "Simple two-state workflow");

            var firstState = new WorkflowState { Id = "first" };
            var lastState = new WorkflowState { Id = "last" };

            result.AddState(firstState);
            result.AddState(lastState);
            result.SetInitialState(firstState);

            firstState.CreateTransition(lastState, "transition");

            return result;
        }
    }
}
