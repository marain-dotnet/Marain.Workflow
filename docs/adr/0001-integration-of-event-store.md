# Integration of an Event Store for audit and "change feed" purposes

## Status

Accepted

## Context

This ADR proposes a solution to several long-standing challenges with Marain.Workflow. They are:

1. The solution does not currently provide any kind of audit log of messages received and/or processed, neither does it provide any kind of history for workflow instances. Some work has been done to address the latter, described in [this GitHub issue](https://github.com/marain-dotnet/Marain.Workflow/issues/132) [and the associated pull request](https://github.com/marain-dotnet/Marain.Workflow/pull/133). This takes the approach of storing the previous version of the workflow instance each time a new version is stored, which addresses the audit log requirement but does not allow external actors to be notified when changes have occurred.

    Without having these features, it is much harder to debug issues with workflow instances as we can't see the actions/events that brought them to their current state.

2. Whilst it is possible to use workflow actions to effect arbitrary changes as part of a transition, experience implementing workflows has led to the conclusion that not all logic should be implemented this way. There is a difference between actions that are required as part of a state change, and actions that could happen as a result of observing those changes once they have taken place. 
   
    For example, in the expenses claim workflow example shown in ["Core Workflow Concepts](../concepts/core-workflow-concepts.md) there is a "submit" transition that takes place when the claimant has submitted their claim for approval. As part of this transition, we might require that (via an action) the appropriate approver is identified and the claim is somehow marked for thir attention. If this cannot be done, the transition cannot succeed.

    However, we may also decide we want a management dashboard to allow us to see statistics about expense claims. At present, this would be implemented by adding actions on the various transitions to update the dashboard. However, this is not core to the task of submitting an expense. As a result we would be happy for it to happen as a result of observing the state change that takes place rather than as part of it. 

    Allowing external services to observe the state changes that have taken place inside the workflow engine and to act on those as needed allows us to keep the workflow definition focussed on its core responsibilities by ensuring it only needs to model directly related activities. This also makes the workflow definitions more maintainable, as there are fewer reasons for them to change.

    To implement this, it would be useful to have some equivalent of Cosmos DB's change feed, or the mechanism by which many Azure services publish events into Event Grid. This would allow other services to react to those events without the need to be explicitly integrated into the workflow.

3. At present, Marain.Workflow is capable of writing extensive log messages into AppInsights. These are hugely useful in diagnosing problems that have led to faulted workflow instances, however there is no easy way to detect when a workflow instance enters a faulted state. With UI-based implementations, this normally manifests as an error message in a user interface followed by a complaint that the user can no longer trigger transitions on a worflow instance (because faulted instances are not permitted to accept triggers).

    The addition of a simple way to detect faulted instances when they occur would greatly improve the ability to proactively address problems in the workflow engine.

These problems can be addressed by introducing the following features:
- persistence of "events" capturing the fact that a workflow instance state change (or status change) has taken place within the workflow engine.
- creation of a "change feed" for the workflow engine, writing out generic events to cover workflow instance creation, transition, completion and faulting.

## Decision

We will update the workflow engine to reimplement workflow instances using the event sourcing pattern. Corvus.EventStore will be integrated to provide the underlying event store. Each workflow instance will be considered an aggregate root.

As always, storage will be siloed by tenant. This will require additional configuration to be supplied to the workflow engine per tenant to determine where event data will be stored.

Further, we will add a means of publishing these events out to interested parties. In the first instance, we will aim to keep this as simple as possible, implementing a webhook based approach using [the CloudEvents standard](https://cloudevents.io/) to publish events to services statically registered with the workflow engine. This will allow us to prove out the event publishing approach quickly and simply.

In the future we can  extended this solution to publish events to an external messaging system such as Azure Event Grid or Service Bus.

## Consequences

### Complexity

This change adds complexity to the codebase in two areas:

- We require additional storage for event data. This needs to be configured as part of tenant onboarding.
- Additional infrastructure in the form of an Event Grid Topic.
  
### Production support

At present, if a workflow instance enters a faulted state then the only way to recover it is manual modification of the workflow instance document in storage.

Once the event store approach is implemented, this becomes impossible; changes can only be effected by adding events to the aggregate. As such, it will be necessary for us to provide some tooling to assist in identification and recovery of faulted instances.

However, this change also makes it easier to identify faulted instances by handling events 

### Subscribers to events must be prepared to handle out of sequence messages and must be idempotent

Whatever mechanism we choose to publish events out of the workflow engine, we will need to adopt the position that:
1. we cannot guarantee any ordering of messages.
1. we cannot guarantee that messages will be published exactly once.

This is due to the possibility that we might publish an event but then fail to persist the fact that it's been dispatched.
