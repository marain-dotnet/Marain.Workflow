@useChildObjects 
@perScenarioContainer

Feature: Workflow Instance
	In order to track progress of items through a workflow
	As a developer
	I want a workflow instance entity to hold that progress

Background:
	Given I have a data catalog workflow definition with Id 'workflow1'
	And I have a context dictionary called 'Context1':
	| Key      | Value  |
	| Context1 | Value1 |
	| Context2 | Value2 |

Scenario: Create a new workflow instance for a workflow
	When I create a new workflow instance
	| InstanceId | WorkflowId | Context    |
	| instance1  | workflow1  | {Context1} |
	Then the workflow instance with Id 'instance1' should have 1 uncommitted event
	And the workflow instance with Id 'instance1' should have the following properties:
	| Property   | Value        |
	| Id         | instance1    |
	| Status     | Initializing |
	| StateId    |              |
	| WorkflowId | workflow1    |
	| Context    | {Context1}   |
	| IsDirty    | true         |

Scenario: IsDirty flag is false when there are no uncommitted events
	Given I have created a new workflow instance
	| InstanceId | WorkflowId | Context    |
	| instance1  | workflow1  | {Context1} |
	And I have persisted the workflow instance with Id 'instance1' to storage
	Then the workflow instance with Id 'instance1' should have 0 uncommitted events
	And the workflow instance with Id 'instance1' should have the following properties:
	| Property   | Value        |
	| IsDirty    | false        |

Scenario: Cannot start a transition when the workflow instance is still Initializing
	Given I have created a new workflow instance
	| InstanceId | WorkflowId | Context    |
	| instance1  | workflow1  | {Context1} |
	When I start the transition 'create' for the workflow instance with Id 'instance1' a trigger of type 'application/vnd.endjin.datacatalog.createcatalogitemtrigger'
	| PropertyName  | Value |
	| CatalogItemId | id1   |
	Then an 'InvalidOperationException' is thrown

Scenario: Faulting the workflow when it is initializing
	Given I have created a new workflow instance
	| InstanceId | WorkflowId | Context    |
	| instance1  | workflow1  | {Context1} |
	And I have persisted the workflow instance with Id 'instance1' to storage
	When I set the workflow instance with Id 'instance1' as faulted with the message 'Intentional fault' and data
	| Key   | Value  |
	| Item1 | Data 1 |
	| Item2 | Data 2 |
	Then the workflow instance with Id 'instance1' should have 1 uncommitted event
	And the workflow instance with Id 'instance1' should have the following properties:
	| Property   | Value        |
	| Id         | instance1    |
	| Status     | Faulted      |
	| StateId    |              |
	| WorkflowId | workflow1    |
	| IsDirty    | true         |

Scenario: Cannot complete initialization by entering a state that is not the workflows initial state
	Given I have created a new workflow instance
	| InstanceId | WorkflowId | Context    |
	| instance1  | workflow1  | {Context1} |
	And I have persisted the workflow instance with Id 'instance1' to storage
	When I set the workflow instance Id 'instance1' as having entered the state 'waiting-for-documentation'
	Then an 'InvalidOperationException' is thrown
	And the workflow instance with Id 'instance1' should have 0 uncommitted events

Scenario: Completing initialization by entering the initial state
	Given I have created a new workflow instance
	| InstanceId | WorkflowId | Context    |
	| instance1  | workflow1  | {Context1} |
	When I set the workflow instance Id 'instance1' as having entered the state 'initializing' with the following context updates:
	| Operation   | Key      | Value          |
	| AddOrUpdate | Context1 | Updated Value1 |
	| AddOrUpdate | Context3 | Value3         |
	| Remove      | Context2 |                |
	Then the workflow instance with Id 'instance1' should have 2 uncommitted events
	And the workflow instance with Id 'instance1' should have the following properties:
	| Property | Value        |
	| Id       | instance1    |
	| Status   | Waiting      |
	| StateId  | initializing |
	| IsDirty  | true         |
	And the workflow instance with Id 'instance1' should have the following context:
	| Key      | Value          |
	| Context1 | Updated Value1 |
	| Context3 | Value3         |

Scenario: Starting a transition when the workflow instance is Waiting
	Given I have created a new workflow instance
	| InstanceId | WorkflowId | Context    |
	| instance1  | workflow1  | {Context1} |
	And I have set the workflow instance Id 'instance1' as having entered the state 'initializing'
	And I have persisted the workflow instance with Id 'instance1' to storage
	When I start the transition 'create' for the workflow instance with Id 'instance1' a trigger of type 'application/vnd.endjin.datacatalog.createcatalogitemtrigger'
	| PropertyName  | Value |
	| CatalogItemId | id1   |
	Then the workflow instance with Id 'instance1' should have 1 uncommitted event
	And the workflow instance with Id 'instance1' should have the following properties:
	| Property | Value                |
	| Id       | instance1            |
	| Status   | ProcessingTransition |
	| StateId  | initializing         |
	| IsDirty  | true                 |

Scenario: Exit the current state when a transition is in progress
	Given I have created a new workflow instance
	| InstanceId | WorkflowId | Context    |
	| instance1  | workflow1  | {Context1} |
	And I have set the workflow instance Id 'instance1' as having entered the state 'initializing'
	And I have started the transition 'create' for the workflow instance with Id 'instance1' a trigger of type 'application/vnd.endjin.datacatalog.createcatalogitemtrigger'
	| PropertyName  | Value |
	| CatalogItemId | id1   |
	And I have persisted the workflow instance with Id 'instance1' to storage
	When I set the workflow instance Id 'instance1' as having exited the current state with the following context updates:
	| Operation   | Key      | Value          |
	| AddOrUpdate | Context1 | Updated Value1 |
	Then the workflow instance with Id 'instance1' should have 1 uncommitted event
	And the workflow instance with Id 'instance1' should have the following properties:
	| Property | Value                |
	| Status   | ProcessingTransition |
	| StateId  |                      |
	| IsDirty  | true                 |
	And the workflow instance with Id 'instance1' should have the following context:
	| Key      | Value          |
	| Context1 | Updated Value1 |
	| Context2 | Value2         |

Scenario: Record transition execution results when a transition is in progress and the previous state has been exited
	Given I have created a new workflow instance
	| InstanceId | WorkflowId | Context    |
	| instance1  | workflow1  | {Context1} |
	And I have set the workflow instance Id 'instance1' as having entered the state 'initializing'
	And I have started the transition 'create' for the workflow instance with Id 'instance1' a trigger of type 'application/vnd.endjin.datacatalog.createcatalogitemtrigger'
	| PropertyName  | Value |
	| CatalogItemId | id1   |
	And I have set the workflow instance Id 'instance1' as having exited the current state with the following context updates:
	| Operation   | Key      | Value          |
	| AddOrUpdate | Context1 | Value1.1 |
	And I have persisted the workflow instance with Id 'instance1' to storage
	When I set the workflow instance Id 'instance1' as having executed transition actions with the following context updates:
	| Operation   | Key      | Value    |
	| AddOrUpdate | Context1 | Value1.2 |
	| AddOrUpdate | Context3 | Value3   |
	| Remove      | Context2 |          |
	Then the workflow instance with Id 'instance1' should have 1 uncommitted event
	And the workflow instance with Id 'instance1' should have the following properties:
	| Property | Value                |
	| Status   | ProcessingTransition |
	| StateId  |                      |
	| IsDirty  | true                 |
	And the workflow instance with Id 'instance1' should have the following context:
	| Key      | Value    |
	| Context1 | Value1.2 |
	| Context3 | Value3   |

Scenario: Enter a new state when a transition is in progress and the previous state has been executed and transition actions recorded
	Given I have created a new workflow instance
	| InstanceId | WorkflowId | Context    |
	| instance1  | workflow1  | {Context1} |
	And I have set the workflow instance Id 'instance1' as having entered the state 'initializing'
	And I have started the transition 'create' for the workflow instance with Id 'instance1' a trigger of type 'application/vnd.endjin.datacatalog.createcatalogitemtrigger'
	| PropertyName  | Value |
	| CatalogItemId | id1   |
	And I have set the workflow instance Id 'instance1' as having exited the current state with the following context updates:
	| Operation   | Key      | Value          |
	| AddOrUpdate | Context1 | Value1.1 |
	And I have set the workflow instance Id 'instance1' as having executed transition actions with the following context updates:
	| Operation   | Key      | Value    |
	| AddOrUpdate | Context1 | Value1.2 |
	| AddOrUpdate | Context3 | Value3   |
	| Remove      | Context2 |          |
	And I have persisted the workflow instance with Id 'instance1' to storage
	When I set the workflow instance Id 'instance1' as having entered the state 'waiting-for-documentation' with the following context updates:
	| Operation   | Key      | Value    |
	| AddOrUpdate | Context4 | Value4.1 |
	| Remove      | Context3 |          |
	Then the workflow instance with Id 'instance1' should have 1 uncommitted event
	And the workflow instance with Id 'instance1' should have the following properties:
	| Property | Value                     |
	| Status   | Waiting                   |
	| StateId  | waiting-for-documentation |
	| IsDirty  | true                      |
	And the workflow instance with Id 'instance1' should have the following context:
	| Key      | Value    |
	| Context1 | Value1.2 |
	| Context4 | Value4.1 |

Scenario: Cannot enter a new state if it is not the target state of the current transition
	Given I have created a new workflow instance
	| InstanceId | WorkflowId | Context    |
	| instance1  | workflow1  | {Context1} |
	And I have set the workflow instance Id 'instance1' as having entered the state 'initializing'
	And I have started the transition 'create' for the workflow instance with Id 'instance1' a trigger of type 'application/vnd.endjin.datacatalog.createcatalogitemtrigger'
	| PropertyName  | Value |
	| CatalogItemId | id1   |
	And I have set the workflow instance Id 'instance1' as having exited the current state with the following context updates:
	| Operation   | Key      | Value          |
	| AddOrUpdate | Context1 | Value1.1 |
	And I have set the workflow instance Id 'instance1' as having executed transition actions with the following context updates:
	| Operation   | Key      | Value    |
	| AddOrUpdate | Context1 | Value1.2 |
	| AddOrUpdate | Context3 | Value3   |
	| Remove      | Context2 |          |
	And I have persisted the workflow instance with Id 'instance1' to storage
	When I set the workflow instance Id 'instance1' as having entered the state 'published'
	Then an 'InvalidOperationException' is thrown
	And the workflow instance with Id 'instance1' should have 0 uncommitted events

Scenario: Cannot start a transition when a transition is already in progress
	Given I have created a new workflow instance
	| InstanceId | WorkflowId | Context    |
	| instance1  | workflow1  | {Context1} |
	And I have set the workflow instance Id 'instance1' as having entered the state 'waiting-for-documentation'
	And I have started the transition 'create' for the workflow instance with Id 'instance1' a trigger of type 'application/vnd.endjin.datacatalog.createcatalogitemtrigger'
	| PropertyName  | Value |
	| CatalogItemId | id1   |
	And I have persisted the workflow instance with Id 'instance1' to storage
	When I start the transition 'create' for the workflow instance with Id 'instance1' a trigger of type 'application/vnd.endjin.datacatalog.createcatalogitemtrigger'
	| PropertyName  | Value |
	| CatalogItemId | id1   |
	Then an 'InvalidOperationException' is thrown
	Then the workflow instance with Id 'instance1' should have 0 uncommitted events

Scenario: Cannot start a transition when a workflow is faulted
	Given I have created a new workflow instance
	| InstanceId | WorkflowId | Context    |
	| instance1  | workflow1  | {Context1} |
	And I have set the workflow instance Id 'instance1' as having entered the state 'initializing'
	And I have set the workflow instance with Id 'instance1' as faulted with the message 'Intentional fault'
	And I have persisted the workflow instance with Id 'instance1' to storage
	When I start the transition 'create' for the workflow instance with Id 'instance1' a trigger of type 'application/vnd.endjin.datacatalog.createcatalogitemtrigger'
	| PropertyName  | Value |
	| CatalogItemId | id1   |
	Then an 'InvalidOperationException' is thrown
	Then the workflow instance with Id 'instance1' should have 0 uncommitted events

Scenario: Cannot exit a state when a transition is not in progress
	Given I have created a new workflow instance
	| InstanceId | WorkflowId | Context    |
	| instance1  | workflow1  | {Context1} |
	And I have set the workflow instance Id 'instance1' as having entered the state 'initializing'
	And I have persisted the workflow instance with Id 'instance1' to storage
	When I set the workflow instance Id 'instance1' as having exited the current state with the following context updates:
	| Operation   | Key      | Value          |
	| AddOrUpdate | Context1 | Updated Value1 |
	Then an 'InvalidOperationException' is thrown
	Then the workflow instance with Id 'instance1' should have 0 uncommitted events

Scenario: Cannot enter a state when a transition is not in progress
	Given I have created a new workflow instance
	| InstanceId | WorkflowId | Context    |
	| instance1  | workflow1  | {Context1} |
	And I have set the workflow instance Id 'instance1' as having entered the state 'initializing'
	And I have persisted the workflow instance with Id 'instance1' to storage
	When I set the workflow instance Id 'instance1' as having entered the state 'waiting-for-documentation'
	Then an 'InvalidOperationException' is thrown
	Then the workflow instance with Id 'instance1' should have 0 uncommitted events

Scenario: Cannot enter a new state prior to exiting the previous state
	Given I have created a new workflow instance
	| InstanceId | WorkflowId | Context    |
	| instance1  | workflow1  | {Context1} |
	And I have set the workflow instance Id 'instance1' as having entered the state 'initializing'
	And I have started the transition 'create' for the workflow instance with Id 'instance1' a trigger of type 'application/vnd.endjin.datacatalog.createcatalogitemtrigger'
	| PropertyName  | Value |
	| CatalogItemId | id1   |
	And I have persisted the workflow instance with Id 'instance1' to storage
	When I set the workflow instance Id 'instance1' as having entered the state 'waiting-for-documentation'
	Then an 'InvalidOperationException' is thrown
	Then the workflow instance with Id 'instance1' should have 0 uncommitted events

Scenario: Cannot record transition action results when a transition is not in progress
	Given I have created a new workflow instance
	| InstanceId | WorkflowId | Context    |
	| instance1  | workflow1  | {Context1} |
	And I have set the workflow instance Id 'instance1' as having entered the state 'initializing'
	And I have persisted the workflow instance with Id 'instance1' to storage
	When I set the workflow instance Id 'instance1' as having executed transition actions with the following context updates:
	| Operation   | Key      | Value    |
	| AddOrUpdate | Context1 | Value1.2 |
	| AddOrUpdate | Context3 | Value3   |
	| Remove      | Context2 |          |
	Then an 'InvalidOperationException' is thrown
	Then the workflow instance with Id 'instance1' should have 0 uncommitted events

Scenario: Cannot record transition actions prior to exiting the previous state
	Given I have created a new workflow instance
	| InstanceId | WorkflowId | Context    |
	| instance1  | workflow1  | {Context1} |
	And I have set the workflow instance Id 'instance1' as having entered the state 'initializing'
	And I have started the transition 'create' for the workflow instance with Id 'instance1' a trigger of type 'application/vnd.endjin.datacatalog.createcatalogitemtrigger'
	| PropertyName  | Value |
	| CatalogItemId | id1   |
	And I have persisted the workflow instance with Id 'instance1' to storage
	When I set the workflow instance Id 'instance1' as having executed transition actions
	Then an 'InvalidOperationException' is thrown
	Then the workflow instance with Id 'instance1' should have 0 uncommitted events

