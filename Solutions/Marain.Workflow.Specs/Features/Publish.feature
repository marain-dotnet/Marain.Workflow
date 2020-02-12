@perFeatureContainer
@setupTenantedCosmosContainers
Feature: Publish an item

@useChildObjects
Scenario: Publish item when it is in the waiting for documentation state and it is complete
	Given I have created and persisted the DataCatalogWorkflow with Id 'dc-workflow'
	And the workflow trigger queue is ready to process new triggers
	And I have created and persisted a new instance with Id 'id1' of the workflow with Id 'dc-workflow' and supplied the following context items
	| Key                              | Value       |
	| Identifier                       | identifier1 |
	| Type                             | t1          |
	| AllowWaitingForDocumentationExit | x           |
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
	And the workflow instance with Id 'id1' should be in the state called 'Published'
	And the following trace messages should be the last messages recorded
	| Message                                   |
	| Exiting state 'Waiting for documentation' |
	| Executing transition 'Publish'            |
	| Entering state 'Published'                |
	