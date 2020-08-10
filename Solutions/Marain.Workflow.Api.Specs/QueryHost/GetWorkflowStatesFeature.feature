@perFeatureContainer
@useWorkflowQueryApi
@useTransientTenant
@useChildObjects
Feature: Get workflow states
	In order to view states for a workflow
	As an external user of the workflow engine
	I want to be able to retrieve states fora given workflow.

Scenario: Request states for a workflow
	Given I have added the workflow 'SimpleExpensesWorkflow' to the workflow store with Id 'simple-expenses-workflow'
	When I get the workflow query path '/{tenantId}/marain/workflow/query/workflows/simple-expenses-workflow/states'
	Then I should have received a 200 status code from the HTTP request
	And the response object should have an array property called '_links.items' containing 5 entries
	And the response object should have an array property called '_embedded.items' containing 5 entries
	And the response object should have a property called '_links.self'

Scenario: Request a workflow that doesn't exist
	When I get the workflow query path '/{tenantId}/marain/workflow/query/workflows/workflow-that-does-not-exist/states'
	Then I should have received a 404 status code from the HTTP request

Scenario: Request states specifying maxItems greater than the number of states
	Given I have added the workflow 'SimpleExpensesWorkflow' to the workflow store with Id 'simple-expenses-workflow'
	When I get the workflow query path '/{tenantId}/marain/workflow/query/workflows/simple-expenses-workflow/states?maxItems=10'
	Then I should have received a 200 status code from the HTTP request
	And the response object should have an array property called '_links.items' containing 5 entries
	And the response object should have an array property called '_embedded.items' containing 5 entries
	And the response object should have a property called '_links.self'
	And the response object should not have a property called '_links.next'

Scenario: Request states specifying maxItems less than the number of states
	Given I have added the workflow 'SimpleExpensesWorkflow' to the workflow store with Id 'simple-expenses-workflow'
	When I get the workflow query path '/{tenantId}/marain/workflow/query/workflows/simple-expenses-workflow/states?maxItems=2'
	Then I should have received a 200 status code from the HTTP request
	And the response object should have an array property called '_links.items' containing 2 entries
	And the response object should have an array property called '_embedded.items' containing 2 entries
	And the response object should have a property called '_links.self'
	And the response object should have a property called '_links.next'

Scenario: Request states using the next link from a previous request
	Given I have added the workflow 'SimpleExpensesWorkflow' to the workflow store with Id 'simple-expenses-workflow'
	And I have requested the workflow query path '/{tenantId}/marain/workflow/query/workflows/simple-expenses-workflow/states?maxItems=2'
	And I have stored the value of the response object property called '_links.next.href' as 'nextLink'
	When I get the workflow query endpoint with the path called 'nextLink'
	Then I should have received a 200 status code from the HTTP request
	And the response object should have an array property called '_links.items' containing 2 entries
	And the response object should have an array property called '_embedded.items' containing 2 entries
	And the response object should have a property called '_links.self'
	And the response object should have a property called '_links.next'

Scenario: Request the final page of states using the next link from a previous request
	Given I have added the workflow 'SimpleExpensesWorkflow' to the workflow store with Id 'simple-expenses-workflow'
	And I have requested the workflow query path '/{tenantId}/marain/workflow/query/workflows/simple-expenses-workflow/states?maxItems=2'
	And I have stored the value of the response object property called '_links.next.href' as 'nextLink'
	And I have requested the workflow query endpoint with the path called 'nextLink'
	And I have stored the value of the response object property called '_links.next.href' as 'nextLink'
	When I get the workflow query endpoint with the path called 'nextLink'
	Then I should have received a 200 status code from the HTTP request
	And the response object should have a property called '_links.items'
	And the response object should have a property called '_embedded.items'
	And the response object should have a property called '_links.self'
	And the response object should not have a property called '_links.next'
