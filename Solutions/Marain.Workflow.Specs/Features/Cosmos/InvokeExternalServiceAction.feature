@perFeatureContainer
@useCosmosStores
@setupTenantedCosmosContainers
Feature: InvokeExternalServiceAction with Cosmos
    In order to be able to define a workflow that invokes external HTTP endpoints
    As a developer defining a workflow
    I want to be able to define a workflow action that invokes an external HTTP endpoint

@externalServiceRequired
Scenario: External action invoked
    Given I have created and persisted a workflow containing an external action with id 'external-action-workflow'
	And I have created and persisted a new instance with Id 'id1' of the workflow with Id 'external-action-workflow' and supplied the following context items
	| Key                              | Value  |
	| include1                         | value1 |
	| include2                         | value2 |
	| dontinclude                      | value3 |
    And the external service will return a 200 status
	And the workflow trigger queue is ready to process new triggers
    When I send a trigger that will execute the action with a trigger id of 'id1'
	And I wait for all triggers to be processed
    Then the action endpoint should have been invoked
    And the Authorization header should be of type Bearer, using a token representing the managed service identity with the resource specified by the action
    And the Content-Type header should be 'application/vnd.marain.workflows.externalservicerequest'
    And the request body WorkflowId should be 'external-action-workflow'
    And the request body WorkflowInstanceId should be 'id1'
    And the request body InvokedById should match the action id
    And the request body Trigger matches the input trigger
    And the request body ContextProperties key 'include1' has the value 'value1'
    And the request body ContextProperties key 'include2' has the value 'value2'
    And the request body ContextProperties has 2 values
	Then the workflow instance with Id 'id1' should have status 'Complete'
	And the workflow instance with Id 'id1' should have 2 change log entries
	And the workflow instance with Id 'id1' should be in the state called 'Done'

@externalServiceRequired
Scenario: External action returns a 500 status code
    Given I have created and persisted a workflow containing an external action with id 'external-action-workflow'
	And I have created and persisted a new instance with Id 'id2' of the workflow with Id 'external-action-workflow' and supplied the following context items
	| Key                              | Value  |
	| include1                         | value1 |
	| include2                         | value2 |
	| dontinclude                      | value3 |
    And the external service will return a 500 status
	And the workflow trigger queue is ready to process new triggers
    When I send a trigger that will execute the action with a trigger id of 'id2'
	And I wait for all triggers to be processed
    Then the action endpoint should have been invoked
	Then the workflow instance with Id 'id2' should have status 'Faulted'
	And the workflow instance with Id 'id2' should have 2 change log entries
	And the workflow instance with Id 'id2' should be in the state called 'Waiting to run'
