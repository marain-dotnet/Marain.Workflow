@perFeatureContainer
@useCosmosStores
@setupTenantedCosmosContainers
Feature: Creating workflow instances
		 In order to ensure that my workflow instances are created correctly
		 And that my workflow can create any supporting data necessary
		 As a workflow designer
		 I want to be able to put conditions and actions on the initial state

Background: 
	Given I have created and persisted the DataCatalogWorkflow with Id 'dc-workflow'
	And the workflow trigger queue is ready to process new triggers
		
Scenario: Create a new instance
	When I create and persist a new instance with Id 'new-instance' of the workflow with Id 'dc-workflow' and supply the following context items
	| Key           | Value       |
	| Identifier    | identifier1 |
	| Type          | t1          |
	And I wait for all triggers to be processed
	Then the workflow instance with Id 'new-instance' should have status 'Waiting'
	And the workflow instance with Id 'new-instance' should be in the state called 'Waiting for documentation'
	And a new data catalog item with Id 'new-instance' should have been added to the data catalog store
	And the data catalog item with Id 'new-instance' should have an Identifier of 'identifier1'
	And the data catalog item with Id 'new-instance' should have a Type of 't1'
	And the following trace messages should have been recorded
	| Message                                     |
	| Entering state 'Waiting for initialization' |
	| Exiting state 'Waiting for initialization'  |
	| Executing transition 'Create catalog item'  |
	| Entering state 'Waiting for documentation'  |

Scenario: Create a new instance with incomplete data
	When I create and persist a new instance with Id 'new-instance-incomplete' of the workflow with Id 'dc-workflow' and supply the following context items
	| Key           | Value       |
	| Type          | t1          |
	And I wait for all triggers to be processed
	Then the workflow instance with Id 'new-instance-incomplete' should have status 'Faulted'
	And a new data catalog item with Id 'new-instance-incomplete' should not have been added to the data catalog store
