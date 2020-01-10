@perFeatureContainer
Feature: SendStartWorkflowInstanceRequest
	In order to tell the workflow engine to carry out actions
	As an external user of the workflow engine
	I want to send a request to start a new workflow instance

Scenario: Send a request to start a new workflow instance
	Given I start a functions instance for the local project 'Endjin.Workflow.Functions.MessageIngestionHost' on port 7071
	Given I start a functions instance for the local project 'Endjin.Operations.Functions.OperationsControlHost' on port 7078
	And I have an object of type "application/vnd.marain.workflows.hosted.startworkflowinstancerequest" called "request"
	| WorkflowId         |
	| target-workflow-id |
	And I am listening for events from the event hub
	When I post the object called "request" to the endpoint "http://localhost:7071/startnewworkflowinstancerequests"
	And wait for up to 3 seconds for incoming events from the event hub
	Then I should have received a 202 status code from the HTTP request
	And I should have received a start new workflow instance message containing JSON data that represents the object called "request"
	And I should not have received an exception from processing events

Scenario: Send multiple requests to start a new workflow instance
	Given I start a functions instance for the local project 'Endjin.Workflow.Functions.MessageIngestionHost' on port 7071
	Given I start a functions instance for the local project 'Endjin.Operations.Functions.OperationsControlHost' on port 7078
	And I have an object of type "application/vnd.marain.workflows.hosted.startworkflowinstancerequest" called "requests"
	| WorkflowId         | WorkflowInstanceId |
	| target-workflow-id | instance-0         |
	| target-workflow-id | instance-1         |
	| target-workflow-id | instance-2         |
	| target-workflow-id | instance-3         |
	| target-workflow-id | instance-4         |
	| target-workflow-id | instance-5         |
	| target-workflow-id | instance-6         |
	| target-workflow-id | instance-7         |
	| target-workflow-id | instance-8         |
	| target-workflow-id | instance-9         |
	And I am listening for events from the event hub
	When I post the object called "requests" to the endpoint "http://localhost:7071/startnewworkflowinstancerequests"
	And wait for up to 3 seconds for incoming events from the event hub
	Then I should have received 10 202 status codes from the HTTP requests
	And I should have received at least 10 start new workflow instance messages containing JSON data that represents the object called "requests"
	And I should not have received an exception from processing events

