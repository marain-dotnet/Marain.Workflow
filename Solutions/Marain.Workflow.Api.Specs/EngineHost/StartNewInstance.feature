@perFeatureContainer
@useWorkflowEngineApi
@useTransientTenant
@useChildObjects
Feature: StartNewInstance
	In order manage a new thing through a workflow
	As an external user of the workflow engine
	I want to be able to start a new instance of a workflow

Scenario: Start a new instance with a specified instance id
	Given I have added the workflow 'SimpleExpensesWorkflow' to the workflow store with Id 'simple-expenses-workflow'
	And The workflow instance store is empty
	And I have a dictionary called 'context'
	| Key        | Value    |
	| Claimant   | J George |
	| CostCenter | GD3724   |
	And I have an object of type 'application/vnd.marain.workflows.hosted.startworkflowinstancerequest' called 'request'
	| WorkflowId               | WorkflowInstanceId | Context   |
	| simple-expenses-workflow | instance           | {context} |
	When I post the object called 'request' to the workflow engine path '/{tenantId}/marain/workflow/engine/workflowinstances'
	Then I should have received a 201 status code from the HTTP request
	And there should be a workflow instance with the id 'instance' in the workflow instance store
	And the workflow instance with id 'instance' should be an instance of the workflow with id 'simple-expenses-workflow'
	And the workflow instance with id 'instance' should have a context dictionary that matches 'context'

Scenario: Start a new instance without specifying an instance id
	Given I have added the workflow 'SimpleExpensesWorkflow' to the workflow store with Id 'simple-expenses-workflow'
	And The workflow instance store is empty
	And I have a dictionary called 'context'
	| Key  | Value |
	| Claimant   | J George |
	| CostCenter | GD3724   |
	And I have an object of type 'application/vnd.marain.workflows.hosted.startworkflowinstancerequest' called 'request'
	| WorkflowId                | Context   |
	| simple-expenses-workflow | {context} |
	When I post the object called 'request' to the workflow engine path '/{tenantId}/marain/workflow/engine/workflowinstances'
	Then I should have received a 201 status code from the HTTP request
	And there should be 1 workflow instance in the workflow instance store

Scenario: Start a new instance with a non-existent workflow id
	Given I have an object of type 'application/vnd.marain.workflows.hosted.startworkflowinstancerequest' called 'request'
	| WorkflowId                           |
	| 4629f9f3-a706-4901-a215-df8313376b52 |
	When I post the object called 'request' to the workflow engine path '/{tenantId}/marain/workflow/engine/workflowinstances'
	Then I should have received a 404 status code from the HTTP request
