@perFeatureContainer
@useWorkflowEngineApi
@useTransientTenant
@useChildObjects
Feature: Update workflow
	In order to modify a workflow definition in the engine
	As an external user of the workflow engine
	I want to be able to store an updated workflow definition

Scenario: Update a workflow definition without supplying an eTag returns a Conflict response
	Given I have added the workflow 'SimpleExpensesWorkflow' to the workflow store with Id 'simple-expenses-workflow-1'
	When I put the workflow called 'SimpleExpensesWorkflow' to the workflow engine path '/{tenantId}/marain/workflow/engine/workflows/simple-expenses-workflow-1'
	Then I should have received a 409 status code from the HTTP request

Scenario: Update a workflow definition with an etag succeeds
	Given I have added the workflow 'SimpleExpensesWorkflow' to the workflow store with Id 'simple-expenses-workflow-2'
	When I put the workflow called 'SimpleExpensesWorkflow' to the workflow engine path '/{tenantId}/marain/workflow/engine/workflows/simple-expenses-workflow-2' with an If-Match header value from the etag of the workflow
	Then I should have received a 200 status code from the HTTP request

Scenario: Attempt to update a workflow definition with an etag that does not match the stored version returns a Conflict response
	Given I have added the workflow 'SimpleExpensesWorkflow' to the workflow store with Id 'simple-expenses-workflow-3'
	When I put the workflow called 'SimpleExpensesWorkflow' to the workflow engine path '/{tenantId}/marain/workflow/engine/workflows/simple-expenses-workflow-3' with '"33a64df551425fcc55e4d42a148795d9f25f89d4"' as the If-Match header value
	Then I should have received a 409 status code from the HTTP request

Scenario: Attempt to store a workflow definition when a definition with the supplied Id does not exist succeeds and creates the new workflow in the store
	Given I have an instance of the workflow 'SimpleExpensesWorkflow' with Id 'simple-expenses-workflow-4'
	When I put the workflow called 'SimpleExpensesWorkflow' to the workflow engine path '/{tenantId}/marain/workflow/engine/workflows/simple-expenses-workflow-4'
	Then I should have received a 200 status code from the HTTP request
	And there should be a workflow with the id 'simple-expenses-workflow-4' in the workflow store
	
Scenario: Attempt to store a workflow definition when the workflow Id in the path doesn't match that in the body returns a Bad Request response
	Given I have added the workflow 'SimpleExpensesWorkflow' to the workflow store with Id 'simple-expenses-workflow-5'
	When I put the workflow called 'SimpleExpensesWorkflow' to the workflow engine path '/{tenantId}/marain/workflow/engine/workflows/the-incorrect-workflow'
	Then I should have received a 400 status code from the HTTP request

