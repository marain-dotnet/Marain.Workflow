﻿@perFeatureContainer
@useWorkflowEngineApi
@useTransientTenant
@useChildObjects
Feature: SendTriggerToInstance
	In order to tell the workflow engine to carry out actions
	As an external user of the workflow engine
	I want to send a trigger to a specific workflow instance

Scenario: Send a trigger
	Given I have a workflow definition with Id 'simple-expenses-workflow' called 'SimpleExpensesWorkflow'
	And I have inserted the workflow called 'SimpleExpensesWorkflow' into the Azure storage workflow store
	And The workflow instance store is empty
	And I have a dictionary called 'context'
	| Key        | Value    |
	| Claimant   | J George |
	| CostCenter | GD3724   |
	And I have started an instance of the workflow 'simple-expenses-workflow' with instance id 'instance' and using context object 'context'
	And I have an object of type 'application/vnd.marain.workflows.hosted.trigger' called 'trigger'
	| TriggerName |
	| Submit      |
	When I post the object called 'trigger' to the workflow engine path '/{tenantId}/marain/workflow/engine/workflowinstances/instance/triggers'
	Then I should have received a 200 status code from the HTTP request
	And the workflow instance with id 'instance' should be in the state with name 'Waiting for approval'

Scenario: Send a trigger with an invalid workflow instance Id
	Given I have an object of type 'application/vnd.marain.workflows.hosted.trigger' called 'trigger'
	| TriggerName |
	| Submit      |
	When I post the object called 'trigger' to the workflow engine path '/{tenantId}/marain/workflow/engine/workflowinstances/a-non-existant-workflow-id/triggers'
	Then I should have received a 404 status code from the HTTP request
