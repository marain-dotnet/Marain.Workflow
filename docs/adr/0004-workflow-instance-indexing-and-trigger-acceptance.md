# Workflow Instance Indexing and Trigger Acceptance

## Status

Proposed

## Context

### Background - current method for matching and processing triggers

As described in ADRs 0001-0003, we are rewriting a section of the workflow engine using the event sourcing pattern. This forces us to consider some aspects of the way in which triggers are processed, especially when workflow instances raise triggers targetting themselves.

At present, the process for handling a trigger is as follows:
- Search for workflow instances whose `Interests` collection has at least one element in common with the trigger `Subjects`.
- For each of those instances
  - Acquire a lease using the workflow instance Id to prevent any other triggers for the same instance from being processed concurrently
  - Process the trigger
  - Persist the updated workflow instance
  - Release the lease

The act of processing a trigger can cause further triggers to be raised. These are raised through the message ingestion endpoint as normal. If a trigger raised in this way targets the workflow instance that caused it to be raised, the leasing process prevents it for being processed until the original trigger processing is complete.

In this specific scenario, it is highly likely that triggers raised in this way will be matched against workflow instance interests that are not up to date, because (assuming they are recieved and processed prior to processing of the original trigger) they will be matched against interests as they were prior to the original trigger being executed. Historically we've been able to ignore this issue because the workflow instance Id is always part of a workflow's `Interests`.

It has caused issues in the past when triggers are raised during workflow creation; originally a workflow instance wasn't persisted until it was fully initialized, meaning that triggers dispatched during initialization would not match against any instances. This issue was resolved by creating and immediately persisting workflow instances in the `Initializing` status (prior to any actions being executed). Any triggers then raised by initial state entry actions would then have a persisted instance to be matched against. This process is summarised as follows:
- Determine the Id of the new workflow instance
- Acquire a lease using the workflow instance Id
- Create and persist the new instance
- Execute initial state entry actions to initialize the instance
- Persist the instance again
- Release the lease, allowing any trigger(s) raised by the state entry actions to be processed

### Changes resulting from the move to event sourcing

The primary challenge from the move to event sourcing is that we no longer have a "workflow instance store" that we can query to match trigger `Subjects` against workflow instance `Interests`. Instead, we have an event store which is not queryable in the way we need.

The "normal" approach to dealing with this when implementing event sourcing is to have separate processes which read committed events from the store and use these to construct and store projections of the data. In our case, the projection we need is an index of workflow instances and their `Interests`. However, this approach introduces a delay between events being written to the store and our index projections being up to date - generally referred to as "eventual consistency".

Eventual consistency has, ultimately, always been something clients needed to take account of when sending events into the engine. Whilst the message ingestion endpoint uses the long running operations pattern to allow a client to know when processing a trigger has completed, this does not give the client visibility of any side effects of processing that trigger - e.g. further triggers being raised. As a result, subsequent triggers intended for the same instance may not be matched against that instance depending on the outcome of any additional processing.

However, we now have the situation that even when the event has been processed, the projection used for matching triggers to instances may not have been updated.

In the scenario where a trigger targets a number of workflow instances based on their interests, the change in approach does not introduce any new problems; there is currently, and will continue to be, a challenge for the client to ensure that they take into account the type of consistency issues described above.

In the scenario where a trigger is intended to target a specific workflow instance using its Id, it doesn't actually matter whether the index is stale or not as long as it's been created. This is because a workflow instance Id is immutable and will always be part of the interests collection so will always be in the index.

The only scenario that is problematic is (as before) during workflow instance creation. We can break this down into two:
- a trigger is raised by an action executed as part of workflow initialization
- a client needs to wait for a workflow instance creation request to complete and then immediately send a trigger.

In the first case, we need to ensure the index entry for the new instance has been created prior to raising the new triggers. The only way of making this the case is to make updating the index and raising the triggers part of the same process. To achieve this, we can implement a way for an action to return a trigger for the workflow engine to create at the appropriate point.

The second case is more problematic. In this scenario it is possible that the trigger will be received before the index contains the `Interests` of the new workflow instance.

This scenario is not considered to be high priority. A scenario where a client needs to wait for workflow instance creation to complete and then immediately send a trigger is possible but unlikely, and would suggest an issue with workflow design. In addition, if it really is necessary for a client to be able to do this, it can be built into the workflow design by introducing an "initializing" workflow state to the start of a workflow. The client can then wait for the instance to leave the "initializing" state, at which point it can be certain that the index exists, and can send any further triggers.

## Decision

At the time of writing (and as part of the changes to move to the event sourcing approach) workflow actions are able to return updates to be made to a workflow instance `Context`. We will extend this to allow the action to return 0 or more triggers that the engine will execute once the workflow instance index of interests is up to date.

We will extend the events raised as part of a transition to include a final `WorkflowInstanceTransitionComplete` event which will include any triggers to be raised once the workflow instance updates are complete and the instance index of interests is updated. We will have an event stream processor which uses this event to update the workflow instance index and dispatches these triggers.