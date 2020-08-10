@perFeatureContainer
@useWorkflowQueryApi
@useTransientTenant
@useChildObjects
Feature: Get workflow
	In order to view a workflow's definition
	As an external user of the workflow engine
	I want to be able to retrieve a workflow.

Scenario: Retrieve workflow
	Given I have added the workflow 'SimpleExpensesWorkflow' to the workflow store with Id 'simple-expenses-workflow'
	When I get the workflow query path '/{tenantId}/marain/workflow/query/workflows/simple-expenses-workflow'
	Then I should have received a 200 status code from the HTTP request
	And the response should contain the the workflow 'SimpleExpensesWorkflow'
	And the response should contain an ETag header

Scenario: Request a workflow that doesn't exist
	When I get the workflow query path '/{tenantId}/marain/workflow/query/workflows/workflow-that-does-not-exist'
	Then I should have received a 404 status code from the HTTP request
