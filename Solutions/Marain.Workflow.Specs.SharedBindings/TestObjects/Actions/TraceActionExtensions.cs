// <copyright file="TraceActionExtensions.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

#pragma warning disable
namespace Marain.Workflows.Specs.TestObjects.Actions
{
    public static class TraceActionExtensions
    {
        public static void AddTraceActionForEntry(this WorkflowState state)
        {
            var action = new TraceAction { Message = $"Entering state '{state.DisplayName}'" };
            state.EntryActions.Add(action);
        }

        public static void AddTraceActionForExit(this WorkflowState state)
        {
            var action = new TraceAction { Message = $"Exiting state '{state.DisplayName}'" };
            state.ExitActions.Add(action);
        }

        public static void AddTraceAction(this WorkflowTransition transition)
        {
            var action = new TraceAction { Message = $"Executing transition '{transition.DisplayName}'" };
            transition.Actions.Add(action);
        }
    }
}
#pragma warning restore