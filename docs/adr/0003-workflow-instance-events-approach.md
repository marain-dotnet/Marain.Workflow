# Workflow Instance events approach

## Status

Proposed

## Context

As described in ADRs 0001 and 0002, we are re-implementing workflow instance tracking using the event sourcing pattern.

As part of this, we need to determine the types of events that the workflow instance will generate as part of initialization, state transitions, error handling and instance completion.

Generally speaking, the lifetime of a workflow instance is as follows:

1. Creation and initialisation, during which a workflow instance has its context initialised and then enters the initial workflow states.
2. Moving through the different workflow states via transitions. Each transition consists of executing exit actions from the current state, executing any actions associated with the transition itself, then executing entry actions for the state being entered. Note that this applies even when the start and end states for the transition are the same.
3. Reaching a final state in the workflow (i.e. one that does not have any transitions associated with it), at which point the instance is given the status `Complete`

We have three choices available to us in terms of how we represent the above as domain events raised by the workflow instance:

### 1. Low verbosity

We represent "creation and initialisation" as one event (`WorkflowInstanceCreated`), "transition" as another (`WorkflowInstanceStateChanged`) and errors as a third (`WorkflowInstanceFaulted`). This would mean that until all actions associated with initialization or a transition were executed, no event would be raised. A sequence of events representing the lifetime of a workflow instance might then look something like this:
| Sequence | Event | Description |
| - | - | - |
| 1 | `WorkflowInstanceCreated` | the workflow is created and entry actions for the | initial state are executed. |
| 2 | `WorkflowInstanceStateChanged` | a successful transition completes |
| 3 | `WorkflowInstanceStateChanged` | a successful transition completes |
| 4 | `WorkflowInstanceStateChanged` | a successful transition completes |
| 5 | `WorkflowInstanceFaulted` | an error occurs whilst executing actions as part of a transition (either exit, transition or entry) and the workflow is put into the `Faulted` status. |

### 2. High verbosity

 We represent execution of every action as an event in its own right, along with state exit and state entry. This would mean we'd need events signifying the start and end of a transition as well. In this case, a sequence of events representing a workflow instance might look like this:

| Sequence | Event | Description |
| - | - | - |
| 1 | `WorkflowInstanceCreated` | The instance is created, but is not yet initialised. At this point it is not in any state. |
| 2 | `WorkflowInstanceActionExecuted` | An entry action for the initial state has executed. |
| 3 | `WorkflowInstanceActionExecuted` | An entry action for the initial state has executed. |
| 4 | `WorkflowInstanceActionExecuted` | An entry action for the initial state has executed. |
| 5 | `WorkflowInstanceStateEntered` | Entry actions for the initial state have all executed and the workflow is now "in" that state. |
| 6 | `WorkflowInstanceInitialized` | Initialization has completed for the workflow instance. |
| 7 | `WorkflowInstanceTransitionStarted`  | An incoming trigger has matched the instance and a transition is about to take place. |
| 8 | `WorkflowInstanceActionExecuted` | An exit action for the current state has been executed. |
| 9 | `WorkflowInstanceActionExecuted` | An exit action for the current state has been executed. |
| 10 | `WorkflowInstanceStateExited` | All exit actions for the current state have executed and the instance is no longer considered to be in that state. |
| 11 | `WorkflowInstanceActionExecuted` | A transition action has executed. |
| 12 | `WorkflowInstanceActionExecuted` | A transition action has executed. |
| 13 | `WorkflowInstanceActionExecuted` | A transition action has executed. |
| 14 | `WorkflowInstanceActionExecuted` | An entry action for the target state has executed. |
| 15 | `WorkflowInstanceActionExecuted` | An entry action for the target state has executed. |
| 16 | `WorkflowInstanceActionExecuted` | An entry action for the target state has executed. |
| 17 | `WorkflowInstanceStateEntered` | Entry actions for the target state have been executed and the workflow instance is now in that state. |
| 18 | `WorkflowInstanceTransitionComplted` | The transition started at sequence number 7 has been completed. |

### 3. Medium verbosity

We take a "middle ground" that doesn't record individual action results, but does represent the component parts of a transition - state exit, transition and state entry. With this option, the above would look like this:

| Sequence | Event | Description |
| - | - | - |
| 1 | `WorkflowInstanceCreated` | The instance is created, but is not yet initialised. At this point it is not in any state. |
| 2 | `WorkflowInstanceStateEntered` | Entry actions for the initial state have all executed and the workflow is now "in" that state. The workflow instance is now initialized. |
| 3 | `WorkflowInstanceTransitionStarted`  | An incoming trigger has matched the instance and a transition is about to take place. |
| 4 | `WorkflowInstanceStateExited` | All exit actions for the current state have executed and the instance is no longer considered to be in that state. |
| 5 | `WorkflowInstanceTransitionExecuted` | Actions associated with the current transition have been executed. |
| 6 | `WorkflowInstanceStateEntered` | Entry actions for the target state have been executed and the workflow instance is now in that state. The transition is now complete. |

## Other concerns

### Execution order for actions

At present, when a set of actions is executed (be it exit, transition or entry), they are processed sequentially. Each action is supplied with the workflow instance which, critically, **may have been modified by the previous action in the set**.

The issue with this is that there is no defined order to the actions. Although they will most likely be executed in the order in which they appear in the workflow definition, we have not formalised this in any way.

If we determine that we should not guarantee execution order for actions, it is reasonable to say that each action should receive an identical copy of the workflow instance context properties, regardless of what changes other actions may have made.

This is the preferred approach and has the advantage that we can execute multiple actions concurrently. It does not preclude implementing ordered execution of actions; we could easily provide an `IWorkflowAction` implemention that provided ordered execution of child actions.

## Decision

