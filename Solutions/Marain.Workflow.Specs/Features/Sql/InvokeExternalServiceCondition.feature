@ignore
@perFeatureContainer
@useSqlStores
@setupTenantedSqlDatabase
Feature: InvokeExternalServiceCondition with SQL
    In order to be able to define a workflow that queries external HTTP endpoints to determine conditions
    As a developer defining a workflow
    I want to be able to define a workflow condition that invokes an external HTTP endpoint

@externalServiceRequired
Scenario: External condition returns true
    Given I have created and persisted a workflow containing an external condition with id 'external-condition-workflow'
	And I have created and persisted a new instance with Id 'id1' of the workflow with Id 'external-condition-workflow' and supplied the following context items
	| Key                              | Value  |
	| include1                         | value1 |
	| include2                         | value2 |
	| dontinclude                      | value3 |
    And the external service will return a 200 status
    And the external service response body will contain 'true'
	And the workflow trigger queue is ready to process new triggers
    When I send a trigger that will execute the condition with a trigger id of 'id1'
	And I wait for all triggers to be processed
    Then the condition endpoint should have been invoked
    And the Authorization header should be of type Bearer, using a token representing the managed service identity with the resource specified by the condition
    And the Content-Type header should be 'application/vnd.marain.workflows.externalservicerequest'
    And the request body WorkflowId should be 'external-condition-workflow'
    And the request body WorkflowInstanceId should be 'id1'
    And the request body InvokedById should match the condition id
    And the request body Trigger matches the input trigger
    And the request body ContextProperties key 'include1' has the value 'value1'
    And the request body ContextProperties key 'include2' has the value 'value2'
    And the request body ContextProperties has 2 values
	Then the workflow instance with Id 'id1' should have status 'Complete'
	And the workflow instance with Id 'id1' should be in the state called 'Done'

@externalServiceRequired
Scenario: External condition returns false
    Given I have created and persisted a workflow containing an external condition with id 'external-condition-workflow'
	And I have created and persisted a new instance with Id 'id2' of the workflow with Id 'external-condition-workflow' and supplied the following context items
	| Key                              | Value  |
	| include1                         | value1 |
	| include2                         | value2 |
	| dontinclude                      | value3 |
    And the external service will return a 200 status
    And the external service response body will contain 'false'
	And the workflow trigger queue is ready to process new triggers
    When I send a trigger that will execute the condition with a trigger id of 'id2'
	And I wait for all triggers to be processed
    Then the condition endpoint should have been invoked
    And the Authorization header should be of type Bearer, using a token representing the managed service identity with the resource specified by the condition
    And the Content-Type header should be 'application/vnd.marain.workflows.externalservicerequest'
    And the request body WorkflowId should be 'external-condition-workflow'
    And the request body WorkflowInstanceId should be 'id2'
    And the request body InvokedById should match the condition id
    And the request body Trigger matches the input trigger
    And the request body ContextProperties key 'include1' has the value 'value1'
    And the request body ContextProperties key 'include2' has the value 'value2'
    And the request body ContextProperties has 2 values
	Then the workflow instance with Id 'id2' should have status 'Waiting'
	And the workflow instance with Id 'id2' should be in the state called 'Waiting to run'

@externalServiceRequired
Scenario: External condition returns a 500 status code
    Given I have created and persisted a workflow containing an external condition with id 'external-condition-workflow'
	And I have created and persisted a new instance with Id 'id3' of the workflow with Id 'external-condition-workflow' and supplied the following context items
	| Key                              | Value  |
	| include1                         | value1 |
	| include2                         | value2 |
	| dontinclude                      | value3 |
    And the external service will return a 500 status
    And the external service response body will contain ''
	And the workflow trigger queue is ready to process new triggers
    When I send a trigger that will execute the condition with a trigger id of 'id3'
	And I wait for all triggers to be processed
    Then the condition endpoint should have been invoked
	Then the workflow instance with Id 'id3' should have status 'Faulted'
	And the workflow instance with Id 'id3' should be in the state called 'Waiting to run'
