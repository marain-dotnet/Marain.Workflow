@perScenarioContainer
@usingCosmosDbNEventStore

Feature: Workflow instance store - CosmosDb NEventStore version

Background: 
	Given I have a data catalog workflow definition with Id 'workflow1'
	And I have a context dictionary called 'Context1':
	| Key      | Value  |
	| Context1 | Value1 |
	| Context2 | Value2 |

Scenario: Store a workflow instance that is in the initialising state
	Given I have created a new workflow instance called 'instance1'
	| InstanceId | WorkflowId | Context    |
	| instance1  | workflow1  | {Context1} |
	When I store the workflow instance called 'instance1'
	Then no exception is thrown
	And the workflow instance called 'instance1' should have the following properties:
	| Property | Value |
	| IsDirty  | false |

Scenario: Store a workflow instance that is initialised
	Given I have created a new workflow instance called 'instance1'
	| InstanceId | WorkflowId | Context    |
	| instance1  | workflow1  | {Context1} |
	And I have set the workflow instance called 'instance1' as having entered the state 'initializing'
	When I store the workflow instance called 'instance1'
	Then no exception is thrown
	And the workflow instance called 'instance1' should have the following properties:
	| Property | Value |
	| IsDirty  | false |

Scenario: Load an initialized workflow instance
	Given I have created a new workflow instance called 'instance1'
	| InstanceId | WorkflowId | Context    |
	| instance1  | workflow1  | {Context1} |
	And I have set the workflow instance called 'instance1' as having entered the state 'initializing'
	And I have stored the workflow instance called 'instance1'
	When I load the workflow instance with Id 'instance1' and call it 'result'
	Then no exception is thrown
	And the workflow instance called 'result' should have the following properties:
	| Property | Value        |
	| Id       | instance1    |
	| Status   | Waiting      |
	| StateId  | initializing |
	| IsDirty  | false        |
	And the workflow instance called 'result' should have the following context:
	| Key      | Value          |
	| Context1 | Value1 |
	| Context2 | Value2 |

Scenario: Load, update and persist a workflow instance
	Given I have created a new workflow instance called 'originalInstance'
	| InstanceId | WorkflowId | Context    |
	| instance1  | workflow1  | {Context1} |
	And I have set the workflow instance called 'originalInstance' as having entered the state 'initializing'
	And I have stored the workflow instance called 'originalInstance'
	And I have loaded the workflow instance with Id 'instance1' and called it 'firstReload'
	And I have started the transition 'create' for the workflow instance called 'firstReload' with a trigger of type 'application/vnd.endjin.datacatalog.createcatalogitemtrigger'
	| PropertyName  | Value |
	| CatalogItemId | id1   |
	And I have set the workflow instance called 'firstReload' as having exited the current state with the following context updates:
	| Operation   | Key      | Value          |
	| AddOrUpdate | Context1 | Value1.1 |
	And I have set the workflow instance called 'firstReload' as having executed transition actions with the following context updates:
	| Operation   | Key      | Value    |
	| AddOrUpdate | Context1 | Value1.2 |
	| AddOrUpdate | Context3 | Value3   |
	| Remove      | Context2 |          |
	And I have set the workflow instance called 'firstReload' as having entered the state 'waiting-for-documentation' with the following context updates:
	| Operation   | Key      | Value    |
	| AddOrUpdate | Context4 | Value4.1 |
	| Remove      | Context3 |          |
	And I have stored the workflow instance called 'firstReload'
	When I load the workflow instance with Id 'instance1' and call it 'result'
	Then the workflow instance called 'result' should have the following properties:
	| Property | Value        |
	| Id       | instance1    |
	| Status   | Waiting      |
	| StateId  | waiting-for-documentation |
	| IsDirty  | false        |
	And the workflow instance called 'result' should have the following context:
	| Key      | Value          |
	| Context1 | Value1.2 |
	| Context4 | Value4.1 |

Scenario: Attempt to persist a workflow instance that has been modified elsewhere since it was loaded
	Given I have created a new workflow instance called 'originalInstance'
	| InstanceId | WorkflowId | Context    |
	| instance1  | workflow1  | {Context1} |
	And I have set the workflow instance called 'originalInstance' as having entered the state 'initializing'
	And I have stored the workflow instance called 'originalInstance'
	And I have loaded the workflow instance with Id 'instance1' and called it 'loadedInstance1'
	And I have loaded the workflow instance with Id 'instance1' and called it 'loadedInstance2'
	And I have started the transition 'create' for the workflow instance called 'loadedInstance1' with a trigger of type 'application/vnd.endjin.datacatalog.createcatalogitemtrigger'
	| PropertyName  | Value |
	| CatalogItemId | id1   |
	And I have started the transition 'create' for the workflow instance called 'loadedInstance2' with a trigger of type 'application/vnd.endjin.datacatalog.createcatalogitemtrigger'
	| PropertyName  | Value |
	| CatalogItemId | id2   |
	And I have stored the workflow instance called 'loadedInstance1'
	When I store the workflow instance called 'loadedInstance2'
	Then an 'InvalidOperationException' is thrown

Scenario: Attempt to persist a new workflow instance when an instance with the same Id already exists
	Given I have created a new workflow instance called 'instance1'
	| InstanceId | WorkflowId | Context    |
	| instance1  | workflow1  | {Context1} |
	And I have set the workflow instance called 'instance1' as having entered the state 'initializing'
	Given I have created a new workflow instance called 'instance2'
	| InstanceId | WorkflowId | Context    |
	| instance1  | workflow1  | {Context1} |
	And I have set the workflow instance called 'instance2' as having entered the state 'initializing'
	And I have stored the workflow instance called 'instance1'
	When I store the workflow instance called 'instance2'
	Then a 'ConcurrencyException' is thrown
