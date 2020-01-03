﻿@setupContainer
Feature: StartNewInstance
	In order manage a new thing through a workflow
	As an external user of the workflow engine
	I want to be able to start a new instance of a workflow

@useChildObjects
Scenario: Start a new instance with a specified instance id
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
	And I have an object of type "application/vnd.marain.workflows.hosted.startworkflowinstancerequest" called "request"
	| WorkflowId               | WorkflowInstanceId | Context   |
	| simple-expenses-workflow | instance           | {context} |
	When I post the object called "request" to the endpoint "http://localhost:7071/startnewworkflowinstancerequests"
	Then I should have received a 202 status code from the HTTP request
	And there should be a workflow instance with the id "instance" in the workflow instance store within 300 seconds
	And the workflow instance with id "instance" should be an instance of the workflow with id "simple-expenses-workflow"
	And the workflow instance with id "instance" should have a context dictionary that matches "context"
