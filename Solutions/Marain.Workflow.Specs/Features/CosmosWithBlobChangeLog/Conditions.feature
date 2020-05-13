@perFeatureContainer
@useCosmosStoresWithBlobChangeLog
@setupTenantedCosmosContainersWithBlobChangeLog
Feature: Conditions with Cosmos and blob change log

@useChildObjects
Scenario: An unmet exit condition on the current state prevents a transition being selected
	Given I have created and persisted the DataCatalogWorkflow with Id 'dc-workflow'
	And the workflow trigger queue is ready to process new triggers
	And I have created and persisted a new instance with Id 'id1' of the workflow with Id 'dc-workflow' and supplied the following context items
	| Key                              | Value       |
	| Identifier                       | identifier1 |
	| Type                             | t1          |
	| AllowPublishedEntry              | x           |
	And I have an object of type 'application/vnd.endjin.datacatalog.catalogitempatchdetails' called 'patch'
	| Id  | Notes         | Description         |
	| id1 | The new notes | The new description |
	And I have sent the workflow engine a trigger of type 'application/vnd.endjin.datacatalog.editcatalogitemtrigger'
	| PatchDetails |
	| {patch}      |
	When I send the workflow engine a trigger of type 'application/vnd.endjin.datacatalog.publishcatalogitemtrigger'
	| CatalogItemId |
	| id1           |
	And I wait for all triggers to be processed
	Then the workflow instance with Id 'id1' should have status 'Waiting'
	And the workflow instance with Id 'id1' should be in the state called 'Waiting for documentation'
	And the workflow instance with Id 'id1' should have 2 change log entries
	And the following trace messages should be the last messages recorded
	| Message                                    |
	| Entering state 'Waiting for documentation' |
	
@useChildObjects
Scenario: An unmet entry condition on a target state prevents a transition being selected
	Given I have created and persisted the DataCatalogWorkflow with Id 'dc-workflow'
	And the workflow trigger queue is ready to process new triggers
	And I have created and persisted a new instance with Id 'id2' of the workflow with Id 'dc-workflow' and supplied the following context items
	| Key                              | Value       |
	| Identifier                       | identifier1 |
	| Type                             | t1          |
	| AllowWaitingForDocumentationExit | x           |
	And I have an object of type 'application/vnd.endjin.datacatalog.catalogitempatchdetails' called 'patch'
	| Id  | Notes         | Description         |
	| id2 | The new notes | The new description |
	And I have sent the workflow engine a trigger of type 'application/vnd.endjin.datacatalog.editcatalogitemtrigger'
	| PatchDetails |
	| {patch}      |
	When I send the workflow engine a trigger of type 'application/vnd.endjin.datacatalog.publishcatalogitemtrigger'
	| CatalogItemId |
	| id2           |
	And I wait for all triggers to be processed
	Then the workflow instance with Id 'id2' should have status 'Waiting'
	And the workflow instance with Id 'id2' should be in the state called 'Waiting for documentation'
	And the workflow instance with Id 'id2' should have 3 change log entries
	And the following trace messages should be the last messages recorded
	| Message                                    |
	| Entering state 'Waiting for documentation' |
	