# The WorkflowInstance Change Log

## Status

Accepted

## Context

We have recently added a `WorkflowInstance` Change Log to the system.

When the `WorkflowEngine` updates the `WorfklowInstance`, it persists it to storage. In parallel with this it also persists a record to the `IWorkflowInstanceChangeLog`. This consists of the updated state of the `WorkflowInstance` together with the `IWorkflowTrigger` that caused the change (or `null` if this was the original initialization of the instance).

## Decision

The Workflow Instance Change Log should be thought of as a structured log of the changes that have been made to a workflow instance, to be used (with the help of tooling) for manual diagnostic and recovery purposes.

It is not a Transaction Log in the sense of a history of actions executed by a database management system used to guarantee ACID properties over crashes or hardware failures.

## Consequences

### Faulting the `WorkflowEngine`

The `WorkflowEngine` is considered to have faulted if _either_ the workflow instance fails to store, _or_ the change long entry fails to store.

This is _unrelated_ to whether a particular `WorkflowInstance` has entered the `Faulted` state - that is "normal operation" for the Engine, and that class of error is handled gracefully.

We are considering the case where it has not been possible for the engine to execute because its own infrastructure has failed (e.g. the WorkflowInstance storage or the WorkflowChangeLog storage become unavailable either temporarily or permanently).

At the moment we have poorly defined behaviour when the `WorkflowEngine` has faulted.

Usually, it will 'soldier on' when the infrastructure next sends it a batch of triggers to process, which is not the safest/most recoverable response to this situation. We should consider the right approach, and we are tracking this discussion in [Issue #139](https://github.com/marain-dotnet/Marain.Workflow/issues/139).

### Appropriate use of the change log

The change log records all the changes to the `WorkflowInstance`. However, in the event of an engine fault, there are several scenarios in which the workflow instance and the state of the log may get out of sync.

1. The workflow instance commits successfully, but the log entry does not
    In this case the Workflow Engine will have faulted, and the last log entry will be *behind* the currently stored workflow instance. Unless you have *also* successfully recorded all the triggers somewhere, it will not be possible to create a complete log entry that corresponds with this change, but you can store the relevant instance (providing the engine has not processed any further triggers for that instance - see "Faulting the `WorkflowEngine`" above)
1. The log entry commits successfully, but the workflow instance does not.
    In this case, the Workflow Engine will have faulted, and the lost log entry will be *ahead* of the currently stored worfklow instance. You can recover the correct workflow instance state by applying the instance in the log to the workflow instance store.
1. Neither the workflow instance nor the log entry commit successfully.
    In this case, it is not possible to recover the log or the entry. You would need to examine the logging to identify the trigger that caused the change (ideally from some separate trigger log), and to replay it into the engine at the head of the queue.

In every case, some or all of the workflow actions may or may not have been successfully executed (depending on whether the workflow instance has transitioned successfully, or faulted). However, that is part of the "normal operation" of the engine, and the ususal caveats about idempotency of your workflow actions apply.
