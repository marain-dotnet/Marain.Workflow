@perFeatureContainer
@useSqlStores
@setupTenantedSqlDatabase
Feature: Delete an item with SQL

@useChildObjects
Scenario: Delete item when it is in the published state
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
	And I have sent the workflow engine a trigger of type 'application/vnd.endjin.datacatalog.publishcatalogitemtrigger'
	| CatalogItemId |
	| id1           |
	When I send the workflow engine a trigger of type 'application/vnd.endjin.datacatalog.deletecatalogitemtrigger'
	| CatalogItemId |
	| id1           |
	And I wait for all triggers to be processed
	Then the workflow instance with Id 'id1' should have status 'Complete'
	And the workflow instance with Id 'id1' should be in the state called 'Deleted'
	And the workflow instance with Id 'id1' should have 5 change log entries
	And the following trace messages should be the last messages recorded
	| Message                       |
	| Exiting state 'Published'     |
	| Executing transition 'Delete' |
	| Entering state 'Deleted'      |