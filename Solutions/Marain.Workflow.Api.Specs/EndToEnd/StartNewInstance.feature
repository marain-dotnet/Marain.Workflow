@perFeatureContainer
@useWorkflowMessageProcessingApi
@useWorkflowEngineApi
@useTransientTenant
@useChildObjects
Feature: StartNewInstance
	In order manage a new thing through a workflow
	As an external user of the workflow engine
	I want to be able to start a new instance of a workflow

Scenario: Start a new instance with a specified instance id
	Given I have a workflow definition with Id 'simple-expenses-workflow' called 'SimpleExpensesWorkflow'
	And I have inserted the workflow called 'SimpleExpensesWorkflow' into the Azure storage workflow store
	And The workflow instance store is empty
	And I have a dictionary called 'context'
	| Key        | Value    |
	| Claimant   | J George |
	| CostCenter | GD3724   |
	And I have an object of type 'application/vnd.marain.workflows.hosted.startworkflowinstancerequest' called 'request'
	| WorkflowId               | WorkflowInstanceId | Context   |
	| simple-expenses-workflow | instance           | {context} |
	When I post the object called 'request' to the workflow message processing path '/{tenantId}/marain/workflow/messageprocessing/startnewworkflowinstancerequests'
	Then I should have received a 202 status code from the HTTP request
	And there should be a workflow instance with the id 'instance' in the workflow instance store within 300 seconds
	And the workflow instance with id 'instance' should be an instance of the workflow with id 'simple-expenses-workflow'
	And the workflow instance with id 'instance' should have a context dictionary that matches 'context'
