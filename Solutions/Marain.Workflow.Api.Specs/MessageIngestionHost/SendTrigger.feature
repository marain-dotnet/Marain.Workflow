@setupContainer
Feature: SendTrigger
	In order to tell the workflow engine to carry out actions
	As an external user of the workflow engine
	I want to send a trigger 

Scenario: Send a trigger
	Given I start a functions instance for the local project 'Endjin.Workflow.Functions.MessageIngestionHost' on port 7071
	And I start a functions instance for the local project 'Endjin.Operations.Functions.OperationsControlHost' on port 7078
	And I have an object of type "application/vnd.marain.workflows.hosted.trigger" called "trigger"
	| TriggerName |
	| TestTrigger |
	And I am listening for events from the event hub
	When I post the object called "trigger" to the endpoint "http://localhost:7071/triggers"
	And wait for up to 3 seconds for incoming events from the event hub
	Then I should have received a 202 status code from the HTTP request
	And I should have received a trigger containing JSON data that represents the object called "trigger"
	And I should not have received an exception from processing events

Scenario: Send multiple triggers
	Given I start a functions instance for the local project 'Endjin.Workflow.Functions.MessageIngestionHost' on port 7071
	And I start a functions instance for the local project 'Endjin.Operations.Functions.OperationsControlHost' on port 7078
	And I have objects of type "application/vnd.marain.workflows.hosted.trigger" called "triggers"
	| TriggerName  |
	| TestTrigger0 |
	| TestTrigger1 |
	| TestTrigger2 |
	| TestTrigger3 |
	| TestTrigger4 |
	| TestTrigger5 |
	| TestTrigger6 |
	| TestTrigger7 |
	| TestTrigger8 |
	| TestTrigger9 |
	And I am listening for events from the event hub
	When I post the objects called "triggers" to the endpoint "http://localhost:7071/triggers"
	And wait for up to 3 seconds for incoming events from the event hub
	Then I should have received 10 202 status codes from the HTTP requests
	And I should have received at least 10 triggers containing JSON data that represents the object called "triggers"
	And I should not have received an exception from processing events
