@perFeatureContainer
@useAzureBlobStore
@setupTenantedBlobStorageContainers
Feature: Blob storage workflow store
	In order to use Azure storage for my workflow definitions
	As a developer
	I want to store and retrieve workflow definitions in Azure Blob storage

Scenario: Store a new workflow definition
	Given I have a workflow definition with Id 'workflow1-1' called 'workflow1-1'
	When I insert the workflow called 'workflow1-1' into the Azure storage workflow store
	Then the request is successful
	And a new blob with Id 'workflow1-1' is created in the container for the current tenant
	And the blob with Id 'workflow1-1' contains the workflow called 'workflow1-1' serialized and encoded using UTF8
	And the eTag returned by the store for the upserted workflow called 'workflow1-1' matches the etag of the blob with Id 'workflow1-1'

	#"the eTag returned by the store for the upserted workflow called '(.*)' matches the etag of the blob with Id '(.*)'"

Scenario: Attempt to store a new workflow definition when a workflow with the same Id already exists
	Given I have a workflow definition with Id 'workflow2-1' called 'workflow2-1'
	And I have inserted the workflow called 'workflow2-1' into the Azure storage workflow store
	And I have a workflow definition with Id 'workflow2-1' called 'workflow2-2'
	When I insert the workflow called 'workflow2-2' into the Azure storage workflow store
	Then a 'WorkflowConflictException' is thrown

Scenario: Retrieve a workflow definition
	Given I have a workflow definition with Id 'workflow3-1' called 'workflow3-1'
	And I have inserted the workflow called 'workflow3-1' into the Azure storage workflow store
	When I request the workflow with Id 'workflow3-1' from the Azure storage workflow store and call it 'workflow3-1-retrieved'
	Then the request is successful
	And the eTag the store returned for get result named 'workflow3-1-retrieved' is the same eTag it returned when upserting workflow 'workflow3-1'

	#"the eTag the store returned for get result named '([^']*)' is the same eTag it returned when upserting workflow '([^']*)'"

Scenario: Attempt to retrieve a workflow definition that does not exist
	When I request the workflow with Id 'made-up-id' from the Azure storage workflow store and call it 'workflow4-1-retrieved'
	Then a 'WorkflowNotFoundException' is thrown

Scenario: Update a workflow definition
	Given I have a workflow definition with Id 'workflow5-1' called 'workflow5-1'
	And I have inserted the workflow called 'workflow5-1' into the Azure storage workflow store
	And I change the description of the workflow definition called 'workflow5-1' to 'A new description'
	When I update the workflow called 'workflow5-1' in the Azure storage workflow store
	Then the request is successful
	And the workflow called 'workflow5-1' has its etag updated to match the etag of the blob with Id 'workflow5-1'

Scenario: Update a workflow definition where the workflow etag does not match the most recent stored version
	Given I have a workflow definition with Id 'workflow6-1' called 'workflow6-1'
	And I have inserted the workflow called 'workflow6-1' into the Azure storage workflow store
	And I change the description of the workflow definition called 'workflow6-1' to 'A new description'
	And I set the etag of the workflow definition called 'workflow6-1' to '"0x8D81C0EA29C6F1E"'
	When I update the workflow called 'workflow6-1' in the Azure storage workflow store
	Then a 'WorkflowPreconditionFailedException' is thrown
