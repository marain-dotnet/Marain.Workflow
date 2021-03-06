# Core Concepts

## Introduction

This document explains the core concepts that are needed to understand the workflow engine.

## Workflow

A workflow is the definition of a process. It is made up of states and transitions between those states. The following diagram shows a workflow representing a simple expense claim process:

```
                              +--------------+  Cancel      +--------------+
                              |              +-------------->              |
                              | In progress  |              |   Deleted    |
                       +------>   (start)    <-------+      |    (end)     |
                       |      |              |       |      |              |
                  Edit |      +--+----+------+       |      +--------------+
                       |         |    |              |
                       +---------+    | Submit       |
                                      |              | Reject
                              +-------v------+       |
                              |              |       |
        Approve (claim >£500) | Waiting for  |       |
        +---------------------+ approval     +-------+
        |                     |              |
        |                     +-------+------+
        |                             |
        |                             | Approve (claim <£500)
        |                             |
+-------v------+              +-------v------+              +--------------+
| Waiting for  |              |              |              |              |
| senior       |   Approve    | Waiting for  |   Paid       |    Paid      |
| approval     +--------------> payment      +-------------->    (end)     |
|              |              ^              |              |              |
+--------------+              +--------------+              +--------------+
```

This workflow contains six states and 8 transitions. The states are represented by boxes, and the transitions by the lines between them.

As you can see from the diagram, the workflow starts with the "In progress" state and finishes when either the "Paid" or "Deleted" states are reached. Transitions allow the item to cycle between the "In Progress" and "Waiting for approval" states.

## Workflow Instance

A workflow instance is a representation of a specific item's progress through the workflow. The workflow above represents the process an expense claim will go through from beginning to end; individual expense claims will be associated with a workflow instance representing their position in that process.

A workflow instance contains the id and version of its parent workflow, the current state of the item in the workflow, and any other useful data that's associated with that instance.

Instances also have a Status. The main statuses are:
- Initializing, which indicates that a workflow instance has been created but is not yet ready to receive triggers 
- Waiting, which indicates that the instance is ready to receive triggers
- Complete, which indicates that the workflow instance has entered a state that has no transitions
- Faulted, which means that an error has occured whilst processing a trigger

## Components of a workflow

### State

A state represents a specific point in the workflow. Normally this represents a point in the process where we are waiting for something to happen. At any point in its lifetime after it's been initialized, a workflow instance will be in exactly one state.

States can have Conditions and Actions associated with them, which allow business rules to be represented in the workflow. An example of this in the above workflow might be that the "Waiting for approval" state has an entry Condition that verifies that all mandatory data is present. This would not normally be used to drive validation; it can be viewed as analagous to a database constraint - a "last line of defence" in ensuring that business rules are being adhered to.

Entry and Exit actions can be used in a similar way - for example, there might be a rule that says when an item reaches the "Waiting for approval" state, the person responsible for approving the claim is notified by email. By making this an entry action we ensure that this always happens.

It's worth noting that it's possible to design your workflow so that all of the conditions and actions are attached to transitions rather than states. However, having the option to use both transition-based actions/conditions and state-based actions/conditions allow you to better model the business rules. As an example, in the diagram above, there are two transitions into the state "Waiting for payment" - one from "Waiting for approval" and one from "Waiting for senior approval". It's reasonable to assume that when an expense claim (i.e. workflow instance) enters the "Awaiting Payment" state, some other process is triggered that ends with a bank transfer. We have two choices as to where to put the action that makes this happen: we can put it on the incoming transitions, or we can make it an entry action to the "Awaiting payment" state.

Adding the action to the incoming transitions will work. But what happens if we add another transition into the "Awaiting payment" state? For example, we might add a state to deal with the scenario where the payment fails, and that state might have a transition back to "Awaiting payment" to indicate that we're retrying the failed payment process. In this situation, we'd need to add the action to start the payment process to that new transition as well. However, if we make it an entry action to the "Awaiting payment" state, we know that regardless of how we enter that state, the action to start the payment process will always run.

This highlights one of the big advantages of choosing to explicitly model workflows in this way: identifying the correct locations for conditions and actions, be they attached to states or transitions, means that as the workflow evolves it is much less likely that we will introduce bugs due to introducing new code paths that are missing constraints or rules.

### Transition

A transition is a definition of a move between two states. It belongs to the state it transitions from, and consists of:
- A target state - this is the workflow state that it will move to when it is applied. For example, the "Submit" transition in the diagram above belongs to the "In progress" state and it's target state is "Waiting for approval".
- A set of conditions - these are a set of tests that must all pass in order for the transition to be executed.
- A set of actions - these are things that will happen as part of the transition.

When the workflow engine receives a trigger, it determines which workflow instances will be given the chance to process that trigger (see "Subjects and Interests", below). The current state of the workflow instance is read and then each transition belonging to that state has its conditions evaluated to determine whether it can process the trigger. The first transition that can is the one that's executed. This process is described in more detail in the "Processing Triggers" section.

### Action

An action is something that happens either as part of a transition, or as a result of entering or leaving a state.

It should be a single, self contained thing that happens - since transitions and states can have multiple actions associated with them, there is no need to combine multiple things into a single action.

Examples of actions are:
- Sending a notification email
- Updating a document in a database
- Submitting an item to a search index
- Raising a trigger into, or creating, another workflow instance

The workflow engine contains two pre-defined actions: LogAction, which writes a diagnostic message based on the workflow instance data, and InvokeExternalServiceAction which sends a POST request to an external endpoint to run the action. If the external service responds with anything other than a success status code (i.e. a 2xx) then an exception is thrown.

If executing an action causes an exception to be thrown, the workflow instance will be placed into the Faulted status.

### Condition

A condition is a test that's evaluated to determine whether a workflow instance is allowed to enter or exit a state, or whether a transition is valid for an incoming trigger.

When evaluated, conditions are provided with the workflow instance and the trigger that has caused the evaluation. It is a simple true/false test, and the workflow engine contains the following pre-defined conditions:
- TriggerContentTypeCondition - validates that the content type of the incoming trigger exactly matches a given content type.
- InvokeExternalServiceCondition - performs a GET or POST request (the method is configurable) to an external endpoint and evaluates the return value, which is expected to be a string containing either "true" or "false". Anything other than a 200 status code results in an exception being thrown, which will cause the workflow instance to become Faulted [**NOTE**: At present, this is incorrect as written; the code currently accepts any 2XX status code without throwing an exception. [This GitHub issue tracks the problem.](https://github.com/marain-dotnet/Marain.Workflow/issues/111)].

If evaluating a condition fails (evaluating to "false" is not considered a failure), the workflow instance will be placed into the Faulted status.

## Trigger

A trigger is a message sent to the workflow engine with the intent of causing a transition to run against a number of workflow instances. At a minimum, triggers have a unique Id, a content type and a list of subjects (see below). When using the hosted workflow service, triggers are also able to have a name and a collection of additional parameters.

When the workflow services recieve a trigger, they attempt to match it up to the workflow instances that should be given the chance to process it. This is primarily done via the list of subjects it contains - see below for more information on Subjects and Interests. Once the list of candidate instances is identified, each is given the opportunity to process the trigger.

It's possible to send a trigger that specifically targets a single workflow instance, and similarly possible to send one that targets all instances in a specific state, or even just all workflow instances.

## Subjects and Interests

As described above, when a trigger is received it could potentially affect any number of workflow instances. This is a common cause of performance issues in workflow engines -  without some means to determine which instances might need to process the trigger, the only recourse is to allow _all_ instances to process it. In a high volume system which may contain millions of workflow instances, this can be a significant barrier to performance.

To address this problem, the Marain Workflow engine uses a system of *Subjects* and *Interests*.

Workflow instances contain a list of Interests. These are data values that can be used to identify triggers of interest to the current state of the workflow instance. The data values themselves are all strings. This list is constructed from:
- all of the interests from each of the current state's entry and exit conditions
- all of the interests from each of the current state's transitions (which in turn is a combination of interests from the transition's conditions)
- the Id of the workflow instance

When raising a trigger into the workflow engine, you can provide a list of Subjects. These are data values that will be matched against the Interests of workflow instances to identify the ones that this trigger is intended for.

Note that the matching process looks for 1 or more matches between the lists of Subjects and Interests; it does not expect them all to match.

This means that if a trigger is intended for a specific workflow instance, maximum efficiency in the matching process is achieved by simply adding the target workflow instance's Id to the trigger's list of subjects.