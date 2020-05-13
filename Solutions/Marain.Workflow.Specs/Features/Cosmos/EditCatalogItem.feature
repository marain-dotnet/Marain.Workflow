@perFeatureContainer
@useCosmosStores
@setupTenantedCosmosContainers
Feature: Edit catalog item with Cosmos

@useChildObjects
Scenario: Edit item in the Waiting for Documentation state
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
	When I send the workflow engine a trigger of type 'application/vnd.endjin.datacatalog.editcatalogitemtrigger'
	| PatchDetails |
	| {patch}      |
	And I wait for all triggers to be processed
	Then the workflow instance with Id 'id1' should have status 'Waiting'
	And the workflow instance with Id 'id1' should be in the state called 'Waiting for documentation'
	And the data catalog item with Id 'id1' should have an Identifier of 'identifier1'
	And the data catalog item with Id 'id1' should have a Type of 't1'
	And the data catalog item with Id 'id1' should have a Description of 'The new description'
	And the data catalog item with Id 'id1' should have Notes of 'The new notes'
	And the workflow instance with Id 'id1' should have 3 change log entries
	And the following trace messages should be the last messages recorded
	| Message                                    |
	| Exiting state 'Waiting for documentation'  |
	| Executing transition 'Edit'                |
	| Entering state 'Waiting for documentation' |

@useChildObjects
Scenario: Edit item in the Published state with a change that would keep the item complete
	Given I have created and persisted the DataCatalogWorkflow with Id 'dc-workflow'
	And the workflow trigger queue is ready to process new triggers
	And I have created and persisted a new instance with Id 'id2' of the workflow with Id 'dc-workflow' and supplied the following context items
	| Key                              | Value       |
	| Identifier                       | identifier1 |
	| Type                             | t1          |
	| AllowWaitingForDocumentationExit | x           |
	| AllowPublishedEntry              | x           |
	And I have an object of type 'application/vnd.endjin.datacatalog.catalogitempatchdetails' called 'InitialPatch'
	| Id  | Notes     | Description     |
	| id2 | The notes | The description |
	And I have an object of type 'application/vnd.endjin.datacatalog.catalogitempatchdetails' called 'TestPatch'
	| Id  | Notes         | Description         |
	| id2 | The new notes | The new description |
	And I have sent the workflow engine a trigger of type 'application/vnd.endjin.datacatalog.editcatalogitemtrigger'
	| PatchDetails   |
	| {InitialPatch} |
	And I have sent the workflow engine a trigger of type 'application/vnd.endjin.datacatalog.publishcatalogitemtrigger'
	| CatalogItemId |
	| id2           |
	When I send the workflow engine a trigger of type 'application/vnd.endjin.datacatalog.editcatalogitemtrigger'
	| PatchDetails |
	| {TestPatch}  |
	And I wait for all triggers to be processed
	Then the workflow instance with Id 'id2' should have status 'Waiting'
	And the workflow instance with Id 'id2' should be in the state called 'Published'
	And the data catalog item with Id 'id2' should have an Identifier of 'identifier1'
	And the data catalog item with Id 'id2' should have a Type of 't1'
	And the data catalog item with Id 'id2' should have a Description of 'The new description'
	And the data catalog item with Id 'id2' should have Notes of 'The new notes'
	And the workflow instance with Id 'id2' should have 5 change log entries
	And the following trace messages should be the last messages recorded
	| Message                                |
	| Exiting state 'Published'              |
	| Executing transition 'Edit (complete)' |
	| Entering state 'Published'             |

@useChildObjects
Scenario: Edit item in the Published state with a change that would make the item incomplete
	Given I have created and persisted the DataCatalogWorkflow with Id 'dc-workflow'
	And the workflow trigger queue is ready to process new triggers
	And I have created and persisted a new instance with Id 'id3' of the workflow with Id 'dc-workflow' and supplied the following context items
	| Key                              | Value       |
	| Identifier                       | identifier1 |
	| Type                             | t1          |
	| AllowWaitingForDocumentationExit | x           |
	| AllowPublishedEntry              | x           |
	And I have an object of type 'application/vnd.endjin.datacatalog.catalogitempatchdetails' called 'InitialPatch'
	| Id  | Notes     | Description     |
	| id3 | The notes | The description |
	And I have an object of type 'application/vnd.endjin.datacatalog.catalogitempatchdetails' called 'TestPatch'
	| Id  | Notes | Description         |
	| id3 |       | The new description |
	And I have sent the workflow engine a trigger of type 'application/vnd.endjin.datacatalog.editcatalogitemtrigger'
	| PatchDetails   |
	| {InitialPatch} |
	And I have sent the workflow engine a trigger of type 'application/vnd.endjin.datacatalog.publishcatalogitemtrigger'
	| CatalogItemId |
	| id3           |
	When I send the workflow engine a trigger of type 'application/vnd.endjin.datacatalog.editcatalogitemtrigger'
	| PatchDetails |
	| {TestPatch}  |
	And I wait for all triggers to be processed
	Then the workflow instance with Id 'id3' should have status 'Waiting'
	And the workflow instance with Id 'id3' should be in the state called 'Waiting for documentation'
	And the data catalog item with Id 'id3' should have an Identifier of 'identifier1'
	And the data catalog item with Id 'id3' should have a Type of 't1'
	And the data catalog item with Id 'id3' should have a Description of 'The new description'
	And the data catalog item with Id 'id3' should have Notes of ''
	And the workflow instance with Id 'id3' should have 5 change log entries
	And the following trace messages should be the last messages recorded
	| Message                                    |
	| Exiting state 'Published'                  |
	| Executing transition 'Edit (incomplete)'   |
	| Entering state 'Waiting for documentation' |
