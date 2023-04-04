@perFeatureContainer
@useWorkflowEngineApi
@useTransientTenant
@useChildObjects
Feature: Create workflow
	In order to add a workflow definition to the engine
	As an external user of the workflow engine
	I want to be able to store a workflow definition

Scenario: Store a workflow definition
	Given I have an instance of the workflow 'SimpleExpensesWorkflow' with Id 'simple-expenses-workflow'
	When I post the workflow called 'SimpleExpensesWorkflow' to the workflow engine path '/{tenantId}/marain/workflow/engine/workflows'
	Then I should have received a 201 status code from the HTTP request
	And there should be a workflow with the id 'simple-expenses-workflow' in the workflow store
	And the response should contain an ETag header

Scenario: Attempt to store a workflow definition when a definition with the specified Id already exists
	Given I have added the workflow 'SimpleExpensesWorkflow' to the workflow store with Id 'simple-expenses-workflow-2'
	When I post the workflow called 'SimpleExpensesWorkflow' to the workflow engine path '/{tenantId}/marain/workflow/engine/workflows'
	Then I should have received a 409 status code from the HTTP request
	