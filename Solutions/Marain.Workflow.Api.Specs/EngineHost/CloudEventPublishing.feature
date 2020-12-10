@perFeatureContainer
@useWorkflowEngineApi
@useTransientTenant
@useChildObjects

Feature: Cloud Event Publishing
	In order to react to changes on workflow instances
	As an external service
	I want to subscribe to events from the workflow engine

Scenario: Subscriber receives event when a workflow instance is created
	Given I have added the workflow 'SimpleExpensesWorkflow' to the workflow store with Id 'simple-expenses-workflow' and event subscriptions
	| ExternalUrl           | MsiAuthenticationResource |
	| http://localhost:7454 |                           |
	And there is an event subscriber listening on port '7454' called 'Subscriber'
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
	And a CloudEvent should have been published to the subscriber called 'Subscriber'
	| Index | PropertyPath                    | Value                                                                  |
	| 0     | source                          | azuresubscriptionid.workflowresourcegroupname.{tenantId}               |
	| 0     | specversion                     | 1.0                                                                    |
	| 0     | subject                         | instance                                                               |
	| 0     | type                            | io.marain.workflow.instance.created                                    |
	| 0     | datacontenttype                 | application/marain.workflows.workflowinstance.creationcloudeventdata   |
	| 0     | data.newState                   | waiting-for-submission                                                 |
	| 0     | data.newStatus                  | waiting                                                                |
	| 0     | data.suppliedContext.Claimant   | J George                                                               |
	| 0     | data.suppliedContext.CostCenter | GD3724                                                                 |
	| 0     | data.newContext.Claimant        | J George                                                               |
	| 0     | data.newContext.CostCenter      | GD3724                                                                 |

Scenario: Single subscriber receives event when workflow state changes
	Given I have added the workflow 'SimpleExpensesWorkflow' to the workflow store with Id 'simple-expenses-workflow' and event subscriptions
	| ExternalUrl           | MsiAuthenticationResource |
	| http://localhost:7454 |                           |
	And there is an event subscriber listening on port '7454' called 'Subscriber'
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
	And CloudEvents should have been published to the subscriber called 'Subscriber'
	| Index | PropertyPath                    | Value                                                                  |
	| 1     | source                          | azuresubscriptionid.workflowresourcegroupname.{tenantId}               |
	| 1     | specversion                     | 1.0                                                                    |
	| 1     | subject                         | instance                                                               |
	| 1     | type                            | io.marain.workflow.instance.transition-completed                       |
	| 1     | datacontenttype                 | application/marain.workflows.workflowinstance.transitioncloudeventdata |
	| 1     | data.previousState              | waiting-for-submission                                                 |
	| 1     | data.previousStatus             | waiting                                                                |
	| 1     | data.newState                   | waiting-for-approval                                                   |
	| 1     | data.newStatus                  | waiting                                                                |
	| 1     | data.transitionId               | submit                                                                 |
	| 1     | data.previousContext.Claimant   | J George                                                               |
	| 1     | data.previousContext.CostCenter | GD3724                                                                 |
	| 1     | data.newContext.Claimant        | J George                                                               |
	| 1     | data.newContext.CostCenter      | GD3724                                                                 |

Scenario: Multiple subscribers receive event when workflow state changes
	Given I have added the workflow 'SimpleExpensesWorkflow' to the workflow store with Id 'simple-expenses-workflow' and event subscriptions
	| ExternalUrl           | MsiAuthenticationResource |
	| http://localhost:7454 |                           |
	| http://localhost:7455 |                           |
	| http://localhost:7456 |                           |
	And there is an event subscriber listening on port '7454' called 'Subscriber1'
	And there is an event subscriber listening on port '7455' called 'Subscriber2'
	And there is an event subscriber listening on port '7456' called 'Subscriber3'
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
	And CloudEvents should have been published to the subscriber called 'Subscriber1'
	| Index | PropertyPath                    | Value                                                                  |
	| 1     | subject                         | instance                                                               |
	| 1     | type                            | io.marain.workflow.instance.transition-completed                       |
	And CloudEvents should have been published to the subscriber called 'Subscriber2'
	| Index | PropertyPath                    | Value                                                                  |
	| 1     | subject                         | instance                                                               |
	| 1     | type                            | io.marain.workflow.instance.transition-completed                       |
	And CloudEvents should have been published to the subscriber called 'Subscriber3'
	| Index | PropertyPath                    | Value                                                                  |
	| 1     | subject                         | instance                                                               |
	| 1     | type                            | io.marain.workflow.instance.transition-completed                       |

Scenario: Workflow instance is not faulted if a subscriber does not return a success status code on publishing
	Given I have added the workflow 'SimpleExpensesWorkflow' to the workflow store with Id 'simple-expenses-workflow' and event subscriptions
	| ExternalUrl           | MsiAuthenticationResource |
	| http://localhost:7454 |                           |
	And there is an event subscriber that will return the status 'InternalServerError' listening on port '7454' called 'Subscriber'
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
	And the workflow instance with id 'instance' should have the status 'Waiting'
	And CloudEvents should have been published to the subscriber called 'Subscriber'
	| Index | PropertyPath                    | Value                                                                  |
	| 1     | subject                         | instance                                                               |
	| 1     | type                            | io.marain.workflow.instance.transition-completed                       |

Scenario: If one subscriber fails, other subscribers still receive the event
	Given I have added the workflow 'SimpleExpensesWorkflow' to the workflow store with Id 'simple-expenses-workflow' and event subscriptions
	| ExternalUrl           | MsiAuthenticationResource |
	| http://localhost:7454 |                           |
	| http://localhost:7455 |                           |
	| http://localhost:7456 |                           |
	And there is an event subscriber listening on port '7454' called 'Subscriber1'
	And there is an event subscriber that will return the status 'InternalServerError' listening on port '7455' called 'Subscriber2'
	And there is an event subscriber listening on port '7456' called 'Subscriber3'
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
	And the workflow instance with id 'instance' should have the status 'Waiting'
	And CloudEvents should have been published to the subscriber called 'Subscriber1'
	| Index | PropertyPath                    | Value                                                                  |
	| 1     | subject                         | instance                                                               |
	| 1     | type                            | io.marain.workflow.instance.transition-completed                       |
	And CloudEvents should have been published to the subscriber called 'Subscriber3'
	| Index | PropertyPath                    | Value                                                                  |
	| 1     | subject                         | instance                                                               |
	| 1     | type                            | io.marain.workflow.instance.transition-completed                       |

Scenario: Cloud event publishing retries 10 times on failure
	Given I have added the workflow 'SimpleExpensesWorkflow' to the workflow store with Id 'simple-expenses-workflow' and event subscriptions
	| ExternalUrl           | MsiAuthenticationResource |
	| http://localhost:7454 |                           |
	And there is an event subscriber that will return the status 'InternalServerError' listening on port '7454' called 'Subscriber'
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
	And CloudEvents should have been published to the subscriber called 'Subscriber'
	| Index | PropertyPath                    | Value                                                                  |
	| 0     | datacontenttype                 | application/marain.workflows.workflowinstance.creationcloudeventdata   |
	| 0     | subject                         | instance                                                               |
	| 1     | datacontenttype                 | application/marain.workflows.workflowinstance.creationcloudeventdata   |
	| 1     | subject                         | instance                                                               |
	| 2     | datacontenttype                 | application/marain.workflows.workflowinstance.creationcloudeventdata   |
	| 2     | subject                         | instance                                                               |
	| 3     | datacontenttype                 | application/marain.workflows.workflowinstance.creationcloudeventdata   |
	| 3     | subject                         | instance                                                               |
	| 4     | datacontenttype                 | application/marain.workflows.workflowinstance.creationcloudeventdata   |
	| 4     | subject                         | instance                                                               |
	| 5     | datacontenttype                 | application/marain.workflows.workflowinstance.creationcloudeventdata   |
	| 5     | subject                         | instance                                                               |
	| 6     | datacontenttype                 | application/marain.workflows.workflowinstance.creationcloudeventdata   |
	| 6     | subject                         | instance                                                               |
	| 7     | datacontenttype                 | application/marain.workflows.workflowinstance.creationcloudeventdata   |
	| 7     | subject                         | instance                                                               |
	| 8     | datacontenttype                 | application/marain.workflows.workflowinstance.creationcloudeventdata   |
	| 8     | subject                         | instance                                                               |
	| 9     | datacontenttype                 | application/marain.workflows.workflowinstance.creationcloudeventdata   |
	| 9     | subject                         | instance                                                               |
