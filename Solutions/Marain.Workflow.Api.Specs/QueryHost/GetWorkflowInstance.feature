@perFeatureContainer
@useWorkflowQueryApi
@useTransientTenant
@useChildObjects

Feature: Get Workflow Instance
	In order to avoid silly mistakes
	As a math idiot
	I want to be told the sum of two numbers

Scenario: Get a workflow instance by Id
	Given I have added the workflow 'SimpleExpensesWorkflow' to the workflow store with Id 'simple-expenses-workflow'
	And I have a dictionary called 'context'
	| Key        | Value    |
	| Claimant   | J George |
	| CostCenter | GD3724   |
	And I have started an instance of the workflow 'simple-expenses-workflow' with instance id 'simple-expenses-instance' and using context object 'context'
	When I get the workflow query path '/{tenantId}/marain/workflow/query/workflowinstances/simple-expenses-instance'
	Then I should have received a 200 status code from the HTTP request
	And the response object should have a property called 'contentType'
	And the response object should have a string property called 'id' with value 'simple-expenses-instance'
	And the response object should have a string property called 'workflowId' with value 'simple-expenses-workflow'
	And the response object should have a string property called 'stateId' with value 'waiting-to-be-submitted'
	And the response object should have an array property called 'interests' containing 4 entries
	And the response object should have a string property called 'context.Claimant' with value 'J George'
	And the response object should have a string property called 'context.CostCenter' with value 'GD3724'

Scenario: Request a workflow instance that doesn't exist
	When I get the workflow query path '/{tenantId}/marain/workflow/query/workflowinstances/this-instance-doesnt-exist'
	Then I should have received a 404 status code from the HTTP request
