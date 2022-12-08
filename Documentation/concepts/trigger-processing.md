# Trigger Processing

When a request is sent into the workflow engine, a number of steps need to happen. This processing happens asynchronously, with the API implementing the long running operation pattern for HTTP endpoints. Specifically, when a request is sent into the workflow engine, the API will respond with an acknowledgement (an HTTP 202 Accepted code) and a URL that can be used to access the status of the operation. This endpoint can be polled to determine when the operation has completed.

When running using the hosting functions provided, the majority of this process is orchestrated using an Azure Durable Function. It's possible to run the workflow engine in-process, but there are some drawbacks and gotchas that are covered later in this document.

Regardless of how the engine is being hosted, the following steps need to take place:

1. Identify the workflow instances that may be able to process the trigger.
2. For each candidate workflow instance, rehydrate it from storage and pass it the trigger to process.
3. If the workflow instance has changed as a result of processing the trigger, persist it back to storage.

## Candidate instance identification

Workflow instances that need to be given the option of processing the trigger are identified using the trigger's list of Subjects and the list of Interests attached to each workflow instance. This is described in more detail in [Core Workflow Concepts - Subjects and Interests](core-workflow-concepts.md)

This results in a list of workflow instance Ids which are then iterated to pass the trigger to each workflow instance.

## Serialization of trigger processing

The workflow engine is intended to support the kind of scale-out that is seen when hosting in serverless functions in the cloud. In this scenario, it is quite possible that there could be hundreds of instances of the workflow engine running concurrently, processing incoming triggers.

In this case it becomes necessary to ensure that an individual workflow instance is never trying to process multiple triggers concurrently in different instances of the engine. In order to do this, we need to make use of a global locking mechanism.

To deal with this problem, we use the shared leasing mechanism provided by Corvus.Leasing. Under the covers, this is configured to use Azure Blob Storage to aquire and release the leases.

When the engine receives a request to apply a trigger to a workflow instance, it will attempt to acquire a lease for that instance. If the lease can't be acquired (because another process has taken a lease for that instance already), then the engine will retry for up to a minute before failing.

## Trigger processing in a workflow instance

When the workflow engine receives a request to apply a trigger to an instance, it takes the following steps:

1. Acquire a lease for the workflow instance, as described above.
1. Retrieve the instance from storage
1. Tell the instance to process the trigger
    1. Retrieve the workflow for the instance from storage.
    1. If the instance is faulted or completed, it can't process the trigger, so processing is finished for this workflow instance. Note that, as with the remainder of these steps, processing completing for an instance due to conditions not being met is **not** considered to be an error scenario (which would put the workflow instance in the Faulted state) because this is a perfectly legitimate outcome.
    1. Exit conditions for the current state of the instance are evaluated to ensure that the workflow instance can transition out of that state. If any of them evaulate to false, processing is complete.
    1. Transitions for the current state are evaluated to find a transition that can be executed as a result of the trigger. This is done by iterating the available transitions and choosing the first one whose conditions all evaluate to true. If no transition is found, processing is complete.
    1. Entry conditions for the transition's target state are evaluated to determine if it is acceptable to move to the new state. If any of the entry conditions evaluate to false, processing is complete.
    1. Exit actions for the current state are executed.
    1. Actions for the transition are executed.
    1. The workflow instance state is updated.
    1. If the workflow instance is now in a state that has no transitions, its Status is set to Completed.
    1. Entry actions for the new state are executed.
1. If an exception was thrown at any point whilst processing the trigger, put the instance into the Faulted status. Note that if external services invoked by `InvokeExternalServiceAction` or `InvokeExternalServiceCondition` return anything other than success status codes, this will result in an exception being thrown which will put the instance into the Faulted status.
1. If the instance has been modified (a transition has been run, or the status has been changed), save it back to storage. 

There is extensive logging during this process, so if an exception is thrown during processing and the workflow instance does end up in a Faulted state, it should be possible to determine the cause of the failure by reviewing log messages.

However, with that said, it is clear from the above that conditions and actions should only throw exceptions as a last resort; it is better to design your workflow to be able to cope with predictable failure conditions and handle them appropriately. Consider the following points when building workflows and writing actions and conditions:

- Prior to raising a trigger, as much validation as needed should take place so that calling code can be confident that trigger processing will succeed.
- If a workflow instance does not invoke any transitions as a result of receiving the trigger (i.e. nothing changes), no exception will be thrown. That means that if you raise a trigger that's targetted at a specific workflow instance via the Subjects collection and that instance does not process the trigger, you will need your own means of determining this.
- In moving a workflow out of the Faulted status, it is likely that some of your actions will be executed again. For example, if a transition between states fails and Faults the workflow, once it is moved back out the Faulted status you are likely to retry the transition. If the transition has multiple actions attached to it, some of these may have already been successfully executed prior to the failure. These actions will be executed again as the transition is retried. To address this, **we strongly recommend that actions be made idempotent** so that no unexpected side effects will happen if they are executed multiple times for a workflow instance.
- Where transitions or states have multiple actions or conditions attached, **the workflow engine does not guarantee the order in which they execute** (or in fact that they will execute in any order at all; they may be executed concurrently). As a result, it is likely that if one action or condition results in an exception, any other actions or conditions attached to the transition or state will still execute. If you need to enforce a specific order, or build "short-circuiting" logic that stops processing after the first failure is encountered then you should construct an aggregate action or condition that adds this behaviour.
- Throwing an exception from a condition or action should be a last resort. If possible add states to your workflow to reflect error scenarios and the means by which they are recovered from, and raise triggers to move workflow instances into those states instead of throwing an exception that will put the instance into the Faulted status. Doing this allows you to make recovering from predictable error conditions part of your workflow.