@setupContainer
Feature: SendTriggerToInstance
	In order to tell the workflow engine to carry out actions
	As an external user of the workflow engine
	I want to send a trigger to a specific workflow instance

@useChildObjects
Scenario: Send a trigger
	Given I start a functions instance for the local project 'Endjin.Operations.Functions.OperationsControlHost' on port 7078
	And I start a functions instance for the local project 'Endjin.Workflow.Functions.MessageIngestionHost' on port 7071
	And I start a functions instance for the local project 'Endjin.Workflow.Functions.EngineHost' on port 7075
	And I start a functions instance for the local project 'Endjin.Workflow.Functions.MessagePreProcessingHost' on port 7073
	And I have added the workflow "SimpleExpensesWorkflow" to the workflow store with Id "simple-expenses-workflow"
	And I have cleared down the workflow instance store
	And I have a dictionary called "context"
	| Key        | Value    |
	| Claimant   | J George |
	| CostCenter | GD3724   |
	And I have started an instance of the workflow "simple-expenses-workflow" with instance id "instance" and using context object "context"
	And I have an object of type "application/vnd.marain.workflows.hosted.trigger" called "trigger"
	| TriggerName |
	| Submit      |
	When I post the object called "trigger" to the endpoint "http://localhost:7071/triggers"
	Then I should have received a 202 status code from the HTTP request
	And the workflow instance with id "instance" should be in the state with name "Waiting for approval" within 180 seconds