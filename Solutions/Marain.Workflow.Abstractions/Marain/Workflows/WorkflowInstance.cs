﻿// <copyright file="WorkflowInstance.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Marain.Workflows.DomainEvents;
    using Marain.Workflows.Internal;

    /// <summary>
    /// The workflow instance.
    /// </summary>
    public class WorkflowInstance
    {
        private readonly WorkflowInstanceSnapshot internalState = new WorkflowInstanceSnapshot();

        private readonly IList<DomainEvent> uncommittedEvents = new List<DomainEvent>();

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowInstance" /> class.
        /// </summary>
        /// <param name="instanceId">The Id of the new instance.</param>
        /// <param name="workflow">The workflow to which this instance belongs.</param>
        /// <param name="context">The context for the workflow instance to create.</param>
        public WorkflowInstance(string instanceId, Workflow workflow, IDictionary<string, string> context)
        {
            this.RaiseEvent(new WorkflowInstanceCreatedEvent(instanceId, this.internalState.Version + 1, workflow.Id, workflow.InitialStateId, context));
        }

        /// <summary>
        /// Gets the workflow instance Id.
        /// </summary>
        public string Id => this.internalState.Id;

        /// <summary>
        /// Gets the Id of the <see cref="Workflow" /> that this is an instance of.
        /// </summary>
        public string WorkflowId => this.internalState.WorkflowId;

        /// <summary>
        /// Gets the Id of the current <see cref="WorkflowState" /> for this instance.
        /// </summary>
        public string StateId => this.internalState.StateId;

        /// <summary>
        /// Gets the status of this instance. A <see cref="WorkflowInstance" /> can only process
        /// new triggers if it's in the <see cref="WorkflowStatus.Waiting" /> status.
        /// </summary>
        public WorkflowStatus Status => this.internalState.Status;

        /// <summary>
        /// Gets a dictionary of useful related pieces of data to be stored with
        /// the workflow instance.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Workflow instances will almost always have related data that are stored independently
        /// of the instance itself. However, to make processing triggers more efficient, you
        /// can choose to add specific pieces of that data.
        /// </para>
        /// </remarks>
        public IImmutableDictionary<string, string> Context => this.internalState.Context;

        /// <summary>
        /// Gets the list of interests for this workflow instance.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This list is automatically updated whenever the instance's current <see cref="StateId" />
        /// changes. The list will be populated by calling <see cref="WorkflowState.GetInterests" />
        /// on the current state and  will always include the value of the <see cref="Id" /> property.
        /// </para>
        /// <para>
        /// This list is intended for use by the <see cref="IWorkflowInstanceStore.GetMatchingWorkflowInstanceIdsForSubjectsAsync(IEnumerable{string}, int, int)" />
        /// method to search for <see cref="WorkflowInstance" />s whose interests match at least one of the
        /// current trigger's subjects (see <see cref="IWorkflowTrigger.GetSubjects" />).
        /// </para>
        /// </remarks>
        public IImmutableList<string> Interests => this.internalState.Interests;

        /// <summary>
        /// Gets a value indicating whether or not this object contains uncommitted changes.
        /// </summary>
        public bool IsDirty => this.uncommittedEvents.Count > 0;

        /// <summary>
        /// Retrieves the list of events that have been created but not persisted to storage.
        /// </summary>
        /// <returns>The list of uncommitted events.</returns>
        public IImmutableList<DomainEvent> GetUncommittedEvents()
        {
            return this.uncommittedEvents.ToImmutableList();
        }

        /// <summary>
        /// Clears the list of uncommitted events. Should only be called once those events have been persisted to
        /// storage.
        /// </summary>
        public void ClearUncommittedEvents()
        {
            this.uncommittedEvents.Clear();
        }

        /// <summary>
        /// Sets the status of the workflow instance to faulted.
        /// </summary>
        /// <param name="errorMessage">A message describing the high-level cause of the fault.</param>
        /// <param name="data">A list of additional data associated with the fault.</param>
        public void SetFaulted(string errorMessage, IDictionary<string, string> data = null)
        {
            this.RaiseEvent(new WorkflowInstanceFaultedEvent(this.Id, this.internalState.Version + 1, errorMessage, data ?? new Dictionary<string, string>()));
        }

        /// <summary>
        /// Indicate that processing a transition is about to start.
        /// </summary>
        /// <param name="workflow">The definition of the workflow that this instance belongs to.</param>
        /// <param name="transition">The transition that is about to be processed.</param>
        /// <param name="trigger">The trigger that caused the transition to begin.</param>
        public void SetTransitionStarted(Workflow workflow, WorkflowTransition transition, IWorkflowTrigger trigger)
        {
            // Are we in a position to start a transition?
            if (this.internalState.Status != WorkflowStatus.Waiting)
            {
                throw new InvalidOperationException($"Cannot start a transition for instance '{this.internalState.Id}' becuase its status is '{this.internalState.Status}'");
            }

            if (workflow.Id != this.internalState.WorkflowId)
            {
                throw new ArgumentException($"The supplied workflow is not the workflow that instance '{this.internalState.Id}' belongs to. Expected workflow with Id '{this.internalState.WorkflowId}', but was supplied '{workflow.Id}'");
            }

            WorkflowState fromState = workflow.GetState(this.internalState.StateId);

            // Ensure that the state that's supplied is the state we're in.
            if (fromState.Id != this.internalState.StateId)
            {
                throw new InvalidOperationException($"Cannot start transition '{transition.Id}' from state '{fromState.Id}' because instance '{this.internalState.Id}' is currently in state '{this.internalState.StateId}'.");
            }

            // Ensure that the transition specified is part of the state.
            if (!fromState.Transitions.Contains(transition))
            {
                throw new ArgumentException($"The supplied transition with Id '{transition.Id}' is not available from the current state '{fromState.Id}'.");
            }

            this.RaiseEvent(new WorkflowInstanceTransitionStartedEvent(
                this.Id,
                this.internalState.Version + 1,
                transition.Id,
                transition.TargetStateId,
                trigger));
        }

        /// <summary>
        /// Indicates that the current state has been exited as part of a transition.
        /// </summary>
        /// <param name="actionResult">The result of invoking exit actions from the current state.</param>
        public void SetStateExited(WorkflowActionResult actionResult)
        {
            // This is only valid if we're currently transitioning.
            if (this.internalState.Status != WorkflowStatus.ProcessingTransition)
            {
                throw new InvalidOperationException($"Cannot exit the state '{this.internalState.StateId}' for instance '{this.internalState.Id}' becuase its status is '{this.internalState.Status}'");
            }

            // TODO: Validate other state properties for this?
            this.RaiseEvent(new WorkflowInstanceStateExitedEvent(
                this.Id,
                this.internalState.Version + 1,
                this.internalState.ActiveTransitionState.TransitionStartedEventVersion,
                this.internalState.StateId,
                actionResult.ContextItemsToAddOrUpdate,
                actionResult.ContextItemsToRemove));
        }

        /// <summary>
        /// Indicates that actions associated with the current transition have been executed.
        /// </summary>
        /// <param name="actionResult">The result of invoking transition actions.</param>
        public void SetTransitionExecuted(WorkflowActionResult actionResult)
        {
            if (this.internalState.Status != WorkflowStatus.ProcessingTransition)
            {
                throw new InvalidOperationException($"Workflow instance '{this.internalState.Id}' cannot execute a transition because its status is '{this.internalState.Status}'. Transition execution can only take place when an instance Status is ProcessingTransition.");
            }

            if (this.internalState.StateId is not null)
            {
                throw new InvalidOperationException($"Workflow instance '{this.internalState.Id}' cannot process transition actions because it has not left its current state '{this.internalState.StateId}'.");
            }

            this.RaiseEvent(new WorkflowInstanceTransitionActionsExecutedEvent(
                this.Id,
                this.internalState.Version + 1,
                this.internalState.ActiveTransitionState.TransitionStartedEventVersion,
                this.internalState.ActiveTransitionState.TransitionId,
                actionResult.ContextItemsToAddOrUpdate,
                actionResult.ContextItemsToRemove));
        }

        /// <summary>
        /// Indicates that actions associated with state entry have been executed successfully. If the instance is
        /// initializing, this indicates that initialization is complete. If it's transitioning, this indicates that
        /// the transition has finished.
        /// </summary>
        /// <param name="enteredState">The state that has been entered by the workflow instance.</param>
        /// <param name="actionResult">The result of invoking the state entry actions.</param>
        public void SetStateEntered(WorkflowState enteredState, WorkflowActionResult actionResult)
        {
            if (this.internalState.Status != WorkflowStatus.Initializing && this.internalState.Status != WorkflowStatus.ProcessingTransition)
            {
                throw new InvalidOperationException($"Workflow instance '{this.internalState.Id}' cannot enter state '{enteredState.Id}' because its status is '{this.internalState.Status}'. State entry can only take place when an instance is Initializing or ProcessingTransition.");
            }

            if (this.internalState.StateId is not null)
            {
                throw new InvalidOperationException($"Workflow instance '{this.internalState.Id}' cannot enter state '{enteredState.Id}' because it has not left its current state '{this.internalState.StateId}'.");
            }

            if (enteredState.Id != this.internalState.ActiveTransitionState.TargetStateId)
            {
                throw new InvalidOperationException($"Workflow instance '{this.internalState.Id}' cannot enter state '{enteredState.Id}' because it is not the target state for the current transition/initialization. The expected state is '{this.internalState.ActiveTransitionState.TargetStateId}'.");
            }

            IEnumerable<string> interests = enteredState.GetInterests(this).Concat(new[] { this.Id });

            this.RaiseEvent(new WorkflowInstanceStateEnteredEvent(
                this.Id,
                this.internalState.Version + 1,
                this.internalState.ActiveTransitionState.TransitionStartedEventVersion,
                enteredState.Id,
                actionResult.ContextItemsToAddOrUpdate,
                actionResult.ContextItemsToRemove,
                interests));
        }

        /// <summary>
        /// Applies the given event to the class.
        /// </summary>
        /// <param name="domainEvent">The event to apply.</param>
        /// <remarks>Not thread safe.</remarks>
        public void ApplyEvent(DomainEvent domainEvent)
        {
            if (domainEvent.SequenceNumber != (this.internalState.Version + 1))
            {
                throw new ArgumentException($"The supplied event with sequence number '{domainEvent.SequenceNumber}' cannot be applied to this aggregate which currently has version number '{this.internalState.Version}'.");
            }

            switch (domainEvent)
            {
                case WorkflowInstanceCreatedEvent createdEvent:
                    this.ApplyEvent(createdEvent);
                    break;

                case WorkflowInstanceFaultedEvent instanceFaulted:
                    this.ApplyEvent(instanceFaulted);
                    break;

                case WorkflowInstanceStateEnteredEvent stateEnteredEvent:
                    this.ApplyEvent(stateEnteredEvent);
                    break;

                case WorkflowInstanceStateExitedEvent stateExitedEvent:
                    this.ApplyEvent(stateExitedEvent);
                    break;

                case WorkflowInstanceTransitionActionsExecutedEvent transitionActionsExecutedEvent:
                    this.ApplyEvent(transitionActionsExecutedEvent);
                    break;

                case WorkflowInstanceTransitionStartedEvent transitionStartedEvent:
                    this.ApplyEvent(transitionStartedEvent);
                    break;

                default:
                    throw new ArgumentException($"Unrecognised domain event type '{domainEvent.ContentType}'.");
            }

            this.internalState.Version = domainEvent.SequenceNumber;
        }

        private void RaiseEvent(DomainEvent newEvent)
        {
            this.ApplyEvent(newEvent);
            this.uncommittedEvents.Add(newEvent);
        }

        private void ApplyEvent(WorkflowInstanceCreatedEvent domainEvent)
        {
            this.internalState.Id = domainEvent.AggregateId;
            this.internalState.WorkflowId = domainEvent.WorkflowId;
            this.internalState.Status = WorkflowStatus.Initializing;
            this.internalState.Context = domainEvent.Context;
            this.internalState.ActiveTransitionState = new WorkflowInstanceActiveTransitionState
            {
               TargetStateId = domainEvent.InitialStateId,
               TransitionStartedEventVersion = 0,
            };
        }

        private void ApplyEvent(WorkflowInstanceFaultedEvent domainEvent)
        {
            this.internalState.Status = WorkflowStatus.Faulted;
        }

        private void ApplyEvent(WorkflowInstanceStateEnteredEvent domainEvent)
        {
            this.internalState.StateId = domainEvent.EnteredStateId;
            this.internalState.Interests = domainEvent.Interests;
            this.internalState.Context = this.BuildNewContext(domainEvent.AddedAndUpdatedContextItems, domainEvent.RemovedContextItems);
            this.internalState.ActiveTransitionState = null;

            // TODO: Set status to Complete if this is the final state.
            this.internalState.Status = WorkflowStatus.Waiting;
        }

        private void ApplyEvent(WorkflowInstanceStateExitedEvent domainEvent)
        {
            this.internalState.StateId = null;
            this.internalState.Context = this.BuildNewContext(domainEvent.AddedAndUpdatedContextItems, domainEvent.RemovedContextItems);
        }

        private void ApplyEvent(WorkflowInstanceTransitionActionsExecutedEvent domainEvent)
        {
            this.internalState.Context = this.BuildNewContext(domainEvent.AddedAndUpdatedContextItems, domainEvent.RemovedContextItems);
        }

        private void ApplyEvent(WorkflowInstanceTransitionStartedEvent domainEvent)
        {
            this.internalState.Status = WorkflowStatus.ProcessingTransition;

            this.internalState.ActiveTransitionState = new WorkflowInstanceActiveTransitionState
            {
                TransitionId = domainEvent.TransitionId,
                TransitionStartedEventVersion = domainEvent.SequenceNumber,
                InitialStateId = this.internalState.StateId,
                TargetStateId = domainEvent.TargetStateId,
                Trigger = domainEvent.Trigger,
            };
        }

        private IImmutableDictionary<string, string> BuildNewContext(IImmutableDictionary<string, string> addedAndUpdatedContextItems, IImmutableList<string> removedContextItems)
        {
            var newContextBuilder = this.internalState.Context.ToImmutableDictionary().ToBuilder();

            foreach (KeyValuePair<string, string> current in addedAndUpdatedContextItems)
            {
                newContextBuilder[current.Key] = current.Value;
            }

            foreach (string current in removedContextItems)
            {
                newContextBuilder.Remove(current);
            }

            return newContextBuilder.ToImmutable();
        }
    }
}