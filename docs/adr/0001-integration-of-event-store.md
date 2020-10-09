# Integration of an Event Store for audit and "change feed" purposes

## Status

Proposed

## Context

This ADR proposes a solution to several long-standing challenges with Marain.Workflow. They are:

1. The solution does not currently provide any kind of audit log of messages received and/or processed, neither does it provide any kind of history for workflow instances. Some work has been done to address the latter, described in [this GitHub issue](https://github.com/marain-dotnet/Marain.Workflow/issues/132) [and the associated pull request](https://github.com/marain-dotnet/Marain.Workflow/pull/133).

    Without having these features, it is much harder to debug issues with workflow instances as we can't see the actions/events that brought them to their current state.

1. Whilst it is possible to use workflow actions to effect arbitrary changes as part of a transition, experience implementing workflows has led to the conclusion that not all logic should be implemented this way. There is a conceptual difference between actions that are required as part of a change, and actions that happen as a result of the change.

    A good example of these latter actions are monitoring and reporting. It is common to want to build out reporting features as part of a solution, and generally advisable to separate out source data for that reporting from data that drives other APIs. Updates to these reporting data stores is secondary to the main concerns of most workflows, so is not a great fit for implementation as a standard workflow action.

    As a result, it would be useful to have some equivalent of Cosmos DB's change feed, or the mechanism by which many Azure services publish events into Event Grid. This would allow other services to react to those events without the need to be explicitly integrated into the workflow.

1. At present, Marain.Workflow is capable of writing extensive log messages into AppInsights. These are hugely useful in diagnosing problems that have led to faulted workflow instances, however there is no easy way to detect when a workflow instance enters a faulted state. With UI-based implementations, this normally manifests as an error message in a user interface followed by a complaint that the user can no longer trigger transitions on a worflow instance (because faulted instances are not permitted to accept triggers).

    The addition of a simple way to detect faulted instances when they occur would greatly improve the ability to proactively address problems in the workflow engine.

These problems can be addressed by introducing the following features:
- persistence of incoming instance creation requests and triggers before dispatching them to workflow instances.
- persistence of "events" capturing the fact that a workflow transition has happened
- creation of a "change feed" for the workflow engine, writing out generic events to cover workflow instance creation, transition, completion and faulting.

It's worth noting that these features could be provided by rewriting Marain.Workflow as an event based system using the event sourcing approach. Whilst this would give us a foundation for addressing the above issues, it is a fundamental change that's best addressed by creating a new "version 2" of Marain.Workflow that's not backwards compatible with existing deployments.

However one core part of an event sourcing-based system, namely the event store, provides a good mechanism for implementing the desired features:

1. Conceptually, an event store is a good fit for the data we are recording. The data we wish to persist is essentially immutable* as it records things that have happened.
2. A core part of an event store is a means to read events out of an "all stream" and process them. This lends itself to publishing these events as a change feed.

\* "Essentially immutable" because there may be GDPR requirements that mean historical event data needs to be scrubbed of personally identifiable information at some point in the future.

## Decision

We will use Corvus.EventStore to persist events at three points in the system:

1. When messages are received by the Message Processing service - "trigger received", "workflow instance creation request received".
2. When a workflow creation request has been processed (or has failed to process correctly) - "workflow instance created", "workflow instance creation failed".
3. When a trigger is processed by a workflow instance (or if the attempt to process the trigger failed and the workflow instance faulted) - "workflow instance transition completed", "workflow instance transition failed".

The first of these forms our audit trail of incoming requests and will be written as events for a single "aggregate root", which is the Marain Tenant within which the requests take place.

The second and third provide our history of change inside the workflow engine. For these, the workflow instance acts as our aggregate root.

As always, storage will be siloed by tenant. This will require additional configuration to be supplied to the workflow engine per tenant to determine where event data will be stored.

Further, we will publish these events out to Azure Event Grid as a change feed for the workflow engine. We will make this a configurable option.

## Consequences

### Data security

Whilst data at rest is siloed by tenant, data we send into Event Grid will not be. This means we can only send the essentials of the event in our events - tenant Id, workflow Id, workflow instance Id, event Id, etc - we cannot send any workflow instance context data or trigger parameters.

However, it is likely that services processing the events will need to access the associated data. We will therefore extend the Workflow API (via the upcoming query API that's being built) to allow access to the underlying data.

If it's possible to read this directly from the event store we will implement in that way; alternatively we may need to persist triggers into blob storage before dispatching the Event Grid event.

### Complexity

This change adds complexity in two areas:

- We require additional storage for event data. This needs to be configured as part of tenant onboarding.
- Additional infrastructure in the form of an Event Grid Topic.