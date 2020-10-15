// <copyright file="WorkflowEngine.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Corvus.Leasing;
    using Corvus.Retry;
    using Marain.Workflows.CloudEvents;
    using Microsoft.Extensions.Logging;

    /// <inheritdoc />
    public class WorkflowEngine : IWorkflowEngine
    {
        private readonly ILeaseProvider leaseProvider;
        private readonly ILogger<IWorkflowEngine> logger;

        private readonly IWorkflowInstanceStore workflowInstanceStore;
        private readonly IWorkflowStore workflowStore;
        private readonly ICloudEventDataPublisher cloudEventPublisher;
        private readonly string cloudEventSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkflowEngine"/> class.
        /// </summary>
        /// <param name="workflowStore">The repository in which to store workflows.</param>
        /// <param name="workflowInstanceStore">The repository in which to store workflow instances.</param>
        /// <param name="leaseProvider">The lease provider.</param>
        /// <param name="cloudEventSource">The source to use when publishing cloud events.</param>
        /// <param name="cloudEventPublisher">The publisher for workflow events.</param>
        /// <param name="logger">A logger for the workflow instance service.</param>
        public WorkflowEngine(
            IWorkflowStore workflowStore,
            IWorkflowInstanceStore workflowInstanceStore,
            ILeaseProvider leaseProvider,
            string cloudEventSource,
            ICloudEventDataPublisher cloudEventPublisher,
            ILogger<IWorkflowEngine> logger)
        {
            this.workflowStore = workflowStore ?? throw new ArgumentNullException(nameof(workflowStore));
            this.workflowInstanceStore =
                workflowInstanceStore ?? throw new ArgumentNullException(nameof(workflowInstanceStore));

            this.leaseProvider = leaseProvider ?? throw new ArgumentNullException(nameof(leaseProvider));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.cloudEventPublisher = cloudEventPublisher ?? throw new ArgumentNullException(nameof(cloudEventPublisher));
            this.cloudEventSource = cloudEventSource;
        }

        /// <inheritdoc/>
        public Task<WorkflowInstance> StartWorkflowInstanceAsync(StartWorkflowInstanceRequest request)
        {
            return Retriable.RetryAsync(() => this.CreateWorkflowInstanceAsync(
                request.WorkflowId,
                null,
                request.WorkflowInstanceId,
                null,
                request.Context));
        }

        /// <inheritdoc/>
        public Task<WorkflowInstance> StartWorkflowInstanceAsync(
            string workflowId,
            string workflowPartitionKey = null,
            string instanceId = null,
            string instancePartitionKey = null,
            IDictionary<string, string> context = null)
        {
            return Retriable.RetryAsync(() =>
            this.CreateWorkflowInstanceAsync(workflowId, workflowPartitionKey, instanceId, instancePartitionKey, context));
        }

        /// <inheritdoc/>
        public Task ProcessTriggerAsync(IWorkflowTrigger trigger, string workflowInstanceId, string partitionKey = null)
        {
            return this.ProcessInstanceWithLeaseAsync(trigger, workflowInstanceId, partitionKey);
        }

        /// <summary>
        /// Retrieves a single <see cref="WorkflowInstance" /> and passes it the trigger
        /// to process.
        /// </summary>
        /// <param name="trigger">The trigger to process.</param>
        /// <param name="instanceId">The Id of the <see cref="WorkflowInstance" /> that will process the trigger.</param>
        /// <param name="partitionKey">The partition key for the instance. If not supplied, the Id will be used.</param>
        /// <returns>A <see cref="Task" /> that will complete when the instance has finished processing the trigger.</returns>
        /// <remarks>
        /// This method retrieves the workflow instance from storage, passes it the trigger
        /// and, if the instance has updated as a result of the trigger, puts it back in
        /// storage.
        /// </remarks>
        private async Task ProcessInstanceAsync(IWorkflowTrigger trigger, string instanceId, string partitionKey)
        {
            WorkflowInstance item = null;
            Workflow workflow = null;

            var workflowEventData = new WorkflowInstanceCloudEventData(instanceId, trigger);
            string workflowEventType = WorkflowEventTypes.TransitionCompleted;

            // What do we want in the event?
            // trigger
            // workflow id
            // workflow instance id
            // previous state
            // new state
            // transition id/name?
            // context value changes/removals
            try
            {
                item = await this.workflowInstanceStore.GetWorkflowInstanceAsync(instanceId, partitionKey).ConfigureAwait(false);
                workflow = await this.workflowStore.GetWorkflowAsync(item.WorkflowId).ConfigureAwait(false);

                workflowEventData.PreviousContext = item.Context.ToDictionary(x => x.Key, x => x.Value);
                workflowEventData.WorkflowId = item.WorkflowId;
                workflowEventData.PreviousState = item.StateId;
                workflowEventData.PreviousStatus = item.Status;

                this.logger.LogDebug($"Accepting trigger {trigger.Id} in instance {item.Id}", trigger, item);

                WorkflowTransition transition = await this.AcceptTriggerAsync(item, trigger).ConfigureAwait(false);

                workflowEventData.TransitionId = transition?.Id;
                workflowEventData.NewState = item.StateId;
                workflowEventData.NewStatus = item.Status;
                workflowEventData.NewContext = item.Context;

                this.logger.LogDebug($"Accepted trigger {trigger.Id} in instance {item.Id}", trigger, item);
            }
            catch (WorkflowInstanceNotFoundException)
            {
                // Bubble this specific exception out as the caller needs to know that they sent through an
                // invalid workflow instance id.
                this.logger.LogError(
                    new EventId(0),
                    $"Unable to locate the specified instance {instanceId} for trigger {trigger.Id}");

                throw;
            }
            catch (Exception ex)
            {
                this.logger.LogError(
                    new EventId(0),
                    ex,
                    $"Error accepting trigger {trigger.Id} in instance {item?.Id}");

                if (item != null)
                {
                    item.Status = WorkflowStatus.Faulted;
                    item.IsDirty = true;
                    workflowEventData.NewStatus = item.Status;
                    workflowEventType = WorkflowEventTypes.InstanceFaulted;
                }
            }
            finally
            {
                if (item?.IsDirty == true)
                {
                    await this.workflowInstanceStore.UpsertWorkflowInstanceAsync(item, partitionKey).ConfigureAwait(false);
                    await this.cloudEventPublisher.PublishWorkflowEventDataAsync(
                        this.cloudEventSource,
                        workflowEventType,
                        item.Id,
                        workflowEventData.ContentType,
                        workflowEventData,
                        workflow.WorkflowEventSubscriptions).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Wraps the <see cref="ProcessInstanceAsync" /> method to use the current <see cref="leaseProvider" />
        /// to take a lease on the workflow instance prior to processing.
        /// </summary>
        /// <param name="trigger">The trigger to process.</param>
        /// <param name="instanceId">The Id of the <see cref="WorkflowInstance" /> that will process the trigger.</param>
        /// <param name="partitionKey">The partition key for the instance. If not supplied, the Id will be used.</param>
        /// <returns>A <see cref="Task" /> that will complete when the instance has finished processing the trigger.</returns>
        /// <remarks>
        /// If another instance of WorkflowEngine already has a lease on the WorkflowInstance,
        /// this method will wait for the lease to become available.
        /// </remarks>
        private Task ProcessInstanceWithLeaseAsync(IWorkflowTrigger trigger, string instanceId, string partitionKey)
        {
            return this.leaseProvider
                .ExecuteWithMutexAsync(_ => this.ProcessInstanceAsync(trigger, instanceId, partitionKey), instanceId);
        }

        /// <summary>
        /// Creates a workflow instance from a workflow.
        /// </summary>
        /// <param name="workflowId">The ID of the workflow.</param>
        /// <param name="workflowPartitionKey">The partition key for the workflow. If not supplied, the Id will be used.</param>
        /// <param name="workflowInstanceId">The id of the workflow instance to create.</param>
        /// <param name="workflowInstancePartitionKey">The partition key for the instance. If not supplied, the Id will be used.</param>
        /// <param name="context">The context for the workflow instance to create.</param>
        /// <returns>The new workflow instance.</returns>
        private async Task<WorkflowInstance> CreateWorkflowInstanceAsync(
            string workflowId,
            string workflowPartitionKey = null,
            string workflowInstanceId = null,
            string workflowInstancePartitionKey = null,
            IDictionary<string, string> context = null)
        {
            var instance = new WorkflowInstance();

            if (!string.IsNullOrEmpty(workflowInstanceId))
            {
                instance.Id = workflowInstanceId;
            }

            Workflow workflow = await this.workflowStore.GetWorkflowAsync(workflowId, workflowPartitionKey).ConfigureAwait(false);
            if (workflow == null)
            {
                throw new WorkflowNotFoundException();
            }

            await this.leaseProvider.ExecuteWithMutexAsync(
                    async _ =>
                    {
                        await this.workflowInstanceStore.UpsertWorkflowInstanceAsync(instance, workflowInstancePartitionKey).ConfigureAwait(false);
                        await this.InitializeInstanceAsync(instance, workflow, context).ConfigureAwait(false);
                        await this.workflowInstanceStore.UpsertWorkflowInstanceAsync(instance, workflowInstancePartitionKey).ConfigureAwait(false);
                    },
                    instance.Id)
                .ConfigureAwait(false);

            return instance;
        }

        /// <summary>
        /// Passes a trigger to an instance and returns the new state.
        /// </summary>
        /// <param name="instance">
        /// The instance that should process the trigger.
        /// </param>
        /// <param name="trigger">
        /// The trigger that should be processed.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> that completes when trigger processing has finished.
        /// </returns>
        /// <remarks>
        /// See the documentation for the <see cref="AcceptTriggerAsync(Workflow, WorkflowState, WorkflowInstance, IWorkflowTrigger)"/>
        /// for a full explanation of the processing steps.
        /// </remarks>
        private async Task<WorkflowTransition> AcceptTriggerAsync(
            WorkflowInstance instance,
            IWorkflowTrigger trigger)
        {
            Workflow workflow = await this.workflowStore.GetWorkflowAsync(instance.WorkflowId).ConfigureAwait(false);
            WorkflowState state = workflow.GetState(instance.StateId);

            if (instance.Status == WorkflowStatus.Faulted)
            {
                return null;
            }

            if (instance.Status == WorkflowStatus.Complete)
            {
                return null;
            }

            return await this.AcceptTriggerAsync(workflow, state, instance, trigger).ConfigureAwait(false);
        }

        /// <summary>
        /// Determines whether an <see cref="WorkflowState" /> is able to accept a trigger,
        /// and processes it if so.
        /// </summary>
        /// <param name="workflow">The workflow that is in operation.</param>
        /// <param name="state">The state that will process the trigger.</param>
        /// <param name="instance">The <see cref="WorkflowInstance" /> that is in this state.</param>
        /// <param name="trigger">The <see cref="IWorkflowTrigger" /> to process.</param>
        /// <returns>
        /// A <see cref="Task" /> that will complete when the trigger has been
        /// processed, or it is determined that the trigger can't be processed
        /// for the given <see cref="WorkflowInstance" /> and <see cref="WorkflowState" />.
        /// </returns>
        /// <remarks>
        /// <para>
        /// This method consists of three parts:
        /// <list type="bullet">
        ///     <item>
        ///         <description>
        ///             Determining if the supplied trigger can be accepted by the
        ///             <see cref="WorkflowInstance" /> in its current state.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <description>
        ///             Executing the actions required to move to the new state.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <description>
        ///             Updating the <see cref="WorkflowInstance" /> with the new state and
        ///             associated interests.
        ///         </description>
        ///     </item>
        /// </list>
        /// </para>
        /// <para>
        /// To determine the transition to use, the following steps are taken:
        /// <list type="bullet">
        ///     <item>
        ///         <description>
        ///             Evaluate the exit conditions of the current state (from
        ///             <see cref="WorkflowState.ExitConditions" />. If any conditions
        ///             evaluate to false, processing ends.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <description>
        ///             Iterate the <see cref="WorkflowState.Transitions" /> collection
        ///             and select the first <see cref="WorkflowTransition" /> whose
        ///             <see cref="WorkflowTransition.Conditions" /> all evaluate to true.
        ///             If no transitions match, then processing ends.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <description>
        ///             Retrieve the target state from the transition, and evaluate its
        ///             entry conditions (from <see cref="WorkflowState.EntryConditions" />.
        ///             If any conditions evaluate to false, processing ends.
        ///         </description>
        ///     </item>
        /// </list>
        /// </para>
        /// <para>
        /// Once it has been determined that the trigger can be processed, actions
        /// from the current state, transition and target state are executed in order:
        /// <list type="bullet">
        ///     <item>
        ///         <description>
        ///             The current state's <see cref="WorkflowState.ExitActions" />.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <description>
        ///             The transition's <see cref="WorkflowTransition.Actions" />
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <description>
        ///             The target state's <see cref="WorkflowState.ExitActions" />.
        ///         </description>
        ///     </item>
        /// </list>
        /// </para>
        /// <para>
        /// Once all actions have been processed, the <see cref="WorkflowInstance" />s status
        /// is set back to <see cref="WorkflowStatus.Waiting" />, if the new current state
        /// contains any transitions. If the new current state doesn't contain any transitions,
        /// there is no way of leaving this state and the workflow status is set to
        /// <see cref="WorkflowStatus.Complete" />. Additionally, the <see cref="WorkflowInstance.IsDirty" />
        /// property is set to true to ensure that the instance is saved by the <see cref="IWorkflowEngine" />.
        /// </para>
        /// </remarks>
        private async Task<WorkflowTransition> AcceptTriggerAsync(
            Workflow workflow,
            WorkflowState state,
            WorkflowInstance instance,
            IWorkflowTrigger trigger)
        {
            bool exitAllowed = await this.CheckConditionsAsync(state.ExitConditions, instance, trigger).ConfigureAwait(false);
            if (!exitAllowed)
            {
                this.logger.LogDebug(
                    $"Exit not permitted from state {state.Id} [{state.DisplayName}] in instance {instance.Id} with trigger {trigger.Id}",
                    state,
                    instance,
                    trigger);
                instance.Status = WorkflowStatus.Waiting;
                return null;
            }

            WorkflowTransition transition = await this.FindTransitionAsync(state.Transitions, instance, trigger).ConfigureAwait(false);
            if (transition == null)
            {
                this.logger.LogDebug(
                    $"No transition found from state {state.Id} [{state.DisplayName}] in instance {instance.Id} with trigger {trigger.Id}",
                    state,
                    instance,
                    trigger);
                instance.Status = WorkflowStatus.Waiting;
                return transition;
            }

            WorkflowState targetState = workflow.GetState(transition.TargetStateId);

            this.logger.LogDebug(
                $"Transition {transition.Id}  [{transition.DisplayName}] found from state {state.Id} [{state.DisplayName}] to state {targetState.Id} [{targetState.DisplayName}] in instance {instance.Id} with trigger {trigger.Id}",
                state,
                instance,
                trigger);

            bool entryAllowed = await this.CheckConditionsAsync(targetState.EntryConditions, instance, trigger)
                                   .ConfigureAwait(false);
            if (!entryAllowed)
            {
                this.logger.LogDebug(
                    $"Entry not permitted into state {targetState.Id} [{targetState.DisplayName}] in instance {instance.Id} with trigger {trigger.Id}",
                    state,
                    instance,
                    trigger);
                instance.Status = WorkflowStatus.Waiting;
                return transition;
            }

            this.logger.LogDebug(
                $"Executing exit actions on transition {transition.Id} from state {state.Id} [{state.DisplayName}] to state {targetState.Id} [{targetState.DisplayName}] in instance {instance.Id} with trigger {trigger.Id}",
                state,
                instance,
                trigger);

            await this.ExecuteAsync(state.ExitActions, instance, trigger).ConfigureAwait(false);

            this.logger.LogDebug(
                $"Executing transition actions on transition {transition.Id} from state {state.Id} [{state.DisplayName}] to state {targetState.Id} [{targetState.DisplayName}] in instance {instance.Id} with trigger {trigger.Id}",
                state,
                instance,
                trigger);
            await this.ExecuteAsync(transition.Actions, instance, trigger).ConfigureAwait(false);

            instance.Status = targetState.Transitions.Count == 0 ? WorkflowStatus.Complete : WorkflowStatus.Waiting;
            instance.SetState(targetState);

            this.logger.LogDebug(
                $"Executing entry actions on transition {transition.Id} from state {state.Id} [{state.DisplayName}] to state {targetState.Id} [{targetState.DisplayName}] in instance {instance.Id} with trigger {trigger.Id}",
                state,
                instance,
                trigger);
            await this.ExecuteAsync(targetState.EntryActions, instance, trigger).ConfigureAwait(false);

            // Then update the instance status, set the new state
            instance.IsDirty = true;
            return transition;
        }

        /// <summary>
        /// Executes all of the actions in the supplied collection against the given
        /// <see cref="WorkflowInstance" /> with the context of the specified trigger.
        /// </summary>
        /// <param name="actions">The list of <see cref="IWorkflowAction" />s to execute.</param>
        /// <param name="instance">The <see cref="WorkflowInstance" /> to execute the actions against.</param>
        /// <param name="trigger">
        /// The <see cref="IWorkflowTrigger" /> that is being processed to cause these actions to be
        /// executed. It is possible for this parameter to be null if the actions being executed
        /// are the entry actions for the initial state of a new <see cref="WorkflowInstance" />.
        /// </param>
        /// <returns>A <see cref="Task" /> that will complete when all actions have been run.</returns>
        /// <remarks>
        /// The actions will be executed sequentially. If an action throws an
        /// exception, subsequent actions will not be executed.
        /// This is a helper method for internal use only. It is used to make code
        /// more readable when executing exit/transition/entry actions.
        /// </remarks>
        private async Task ExecuteAsync(
            IEnumerable<IWorkflowAction> actions,
            WorkflowInstance instance,
            IWorkflowTrigger trigger)
        {
            foreach (IWorkflowAction obj in actions)
            {
                await obj.ExecuteAsync(instance, trigger).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Finds the first <see cref="WorkflowTransition"/> in the supplied
        /// list that can accept the specified <see cref="IWorkflowTrigger"/>.
        /// </summary>
        /// <param name="transitions">The transitions to evaluate.</param>
        /// <param name="instance">The <see cref="WorkflowInstance"/> to which the transition belongs.</param>
        /// <param name="trigger">The trigger that is currently being processed.</param>
        /// <returns>
        /// A <see cref="Task"/> whose result will be the first transition
        /// from the list that can accept the trigger. If none of the transitions
        /// can accept the trigger, null is returned.
        /// </returns>
        private async Task<WorkflowTransition> FindTransitionAsync(
            IEnumerable<WorkflowTransition> transitions,
            WorkflowInstance instance,
            IWorkflowTrigger trigger)
        {
            foreach (WorkflowTransition transition in transitions)
            {
                this.logger.LogDebug($"Considering transition {transition.Id} [{transition.DisplayName}]");
                if (await this.CheckConditionsAsync(transition.Conditions, instance, trigger).ConfigureAwait(false))
                {
                    this.logger.LogDebug($"Accepted transition {transition.Id} [{transition.DisplayName}]");
                    return transition;
                }

                this.logger.LogDebug($"Rejected transition {transition.Id} [{transition.DisplayName}]");
            }

            return null;
        }

        /// <summary>
        /// Evaluates all of the conditions in the supplied collection against the given
        /// <see cref="WorkflowInstance" /> with the context of the specified trigger.
        /// </summary>
        /// <param name="conditions">The list of <see cref="IWorkflowCondition" />s to evaluate.</param>
        /// <param name="instance">The <see cref="WorkflowInstance" /> to evaluate the conditions actions against.</param>
        /// <param name="trigger">
        /// The <see cref="IWorkflowTrigger" /> that is being processed to cause these conditions to be
        /// evaluated. It is possible for this parameter to be null if the conditions being evaluated
        /// are the entry conditions for the initial state of a new <see cref="WorkflowInstance" />.
        /// </param>
        /// <returns>A <see cref="Task" /> that will complete when all conditions have been evaluated.</returns>
        /// <remarks>
        /// The actions will be executed sequentially. If a condition is not met, remaining
        /// conditions will not be evaluated.
        /// This is a helper method for internal use only. It is used to make code
        /// more readable when evaluating exit/transition/entry conditions.
        /// </remarks>
        private async Task<bool> CheckConditionsAsync(
            IEnumerable<IWorkflowCondition> conditions,
            WorkflowInstance instance,
            IWorkflowTrigger trigger)
        {
            foreach (IWorkflowCondition condition in conditions)
            {
                this.logger.LogDebug($"Evaluating condition {condition.Id} {condition.GetType().Name}");

                if (!await condition.EvaluateAsync(instance, trigger).ConfigureAwait(false))
                {
                    this.logger.LogDebug($"Condition false: {condition.Id} {condition.GetType().Name}");
                    return false;
                }

                this.logger.LogDebug($"Condition true: {condition.Id} {condition.GetType().Name}");
            }

            return true;
        }

        /// <summary>
        /// Initializes a <see cref="WorkflowInstance" />. See Remarks for full details of this process.
        /// </summary>
        /// <param name="instance">The workflow instance.</param>
        /// <param name="workflow">The <see cref="Workflow" /> that this is an instance of.</param>
        /// <param name="context">The dictionary of context values that was supplied when this instance was created.</param>
        /// <returns>A <see cref="Task" /> that completes when the instance is initialised.</returns>
        /// <remarks>
        /// <para>
        /// Initialization includes the following steps:
        /// - Setting the instance's <see cref="WorkflowInstance.Context" /> property to the supplied dictionary.
        /// - Setting the state of this instance to the workflow's initial state.
        /// - Validating the entry conditions of the initial state.
        /// - Executing the entry actions of the initial state.
        /// </para>
        /// <para>
        /// It is possible for an action executed as part of initialization to cause an
        /// <see cref="IWorkflowTrigger" /> to be created. If an action does do this, it should
        /// add it to the current <see cref="IWorkflowMessageQueue" />. As a result of this it
        /// is vitally important that the code creating and Initializing a workflow instance
        /// takes a shared lease at the earliest possible moment.
        /// </para>
        /// </remarks>
        private async Task InitializeInstanceAsync(WorkflowInstance instance, Workflow workflow, IDictionary<string, string> context = null)
        {
            instance.WorkflowId = workflow.Id;
            instance.Context = context;

            WorkflowState workflowState = workflow.GetInitialState();

            bool entryAllowed = await this.CheckConditionsAsync(workflowState.EntryConditions, instance, null).ConfigureAwait(false);

            if (!entryAllowed)
            {
                this.logger.LogDebug(
                    $"Initialization not permitted to state {workflowState.Id} [{workflowState.DisplayName}] in instance {instance.Id}",
                    workflowState,
                    this);
                instance.Status = WorkflowStatus.Faulted;

                return;
            }

            instance.SetState(workflowState);
            instance.Status = WorkflowStatus.Waiting;

            this.logger.LogDebug(
                $"Executing entry actions on state {workflowState.Id} [{workflowState.DisplayName}]",
                workflowState,
                this);

            await this.ExecuteAsync(workflowState.EntryActions, instance, null).ConfigureAwait(false);
        }
    }
}