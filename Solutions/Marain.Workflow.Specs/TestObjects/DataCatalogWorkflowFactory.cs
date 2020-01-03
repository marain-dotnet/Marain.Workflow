// <copyright file="DataCatalogWorkflowFactory.cs" company="Endjin Limited">
// Copyright (c) Endjin Limited. All rights reserved.
// </copyright>

namespace Marain.Workflows.Specs.TestObjects
{
    using Marain.Workflows.Specs.TestObjects.Actions;
    using Marain.Workflows.Specs.TestObjects.Conditions;
    using Marain.Workflows.Specs.TestObjects.Subjects;
    using Marain.Workflows.Specs.TestObjects.Triggers;

    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    ///     The data catalog workflow factory.
    /// </summary>
    public static class DataCatalogWorkflowFactory
    {
        /// <summary>
        /// Creates an instance of a ficticious workflow for testing purposes.
        /// </summary>
        /// <param name="id">The id to give the new workflow.</param>
        /// <param name="workflowMessageQueue">The message queue to send workflow messages via.</param>
        /// <returns>
        /// The new <see cref="Workflow" />.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The workflow generated is shown in the following diagram:
        ///     </para>
        ///     <para>
        ///         <c>
        ///                              Start
        ///                                |
        ///                                |
        ///                        +-------v-------+
        ///                        |               |
        ///                        | Initializing  |
        ///                        |               |
        ///                        +-------+-------+
        ///                                |
        ///                                | Create
        ///                                |
        ///                        +-------v-------+
        ///                        |               |
        ///                        | Waiting for   |
        ///                 +------+ documentation |
        ///                 |      |               |
        ///                 |      +-------+-------+
        ///                 |              |
        ///           Edit                 | Publish
        ///           (incomplete)         |
        ///                 |      +-------v-------+               +---------------+
        ///                 |      |               |               |               |
        ///                 +------+   Published   |  Deprecate    |  Deprecated   |
        ///                        |               +--------------->    (end)      |
        ///                 +------>               |               |               |
        ///                 |      +--+---------+--+               +---------------+
        ///                 |         |         |
        ///            Edit +---------+         | Delete
        ///            (complete)               |
        ///                        +------------v--+
        ///                        |               |
        ///                        |    Deleted    |
        ///                        |     (end)     |
        ///                        |               |
        ///                        +---------------+
        ///         </c>.
        ///     </para>
        ///     <para>
        ///         To assist with testing it has the following notable points:
        ///         <list type="bullet">
        ///             <item>
        ///                 <description>
        ///                     The initial state, "Initializing" will validate the context properties
        ///                     and self-trigger a transition to "Waiting for documentation.
        ///                 </description>
        ///             </item>
        ///             <item>
        ///                 <description>
        ///                     Every state has a TraceAction added to its entry and exit actions lists
        ///                     so that tests can verify the expected actions were called in the expected
        ///                     order.
        ///                 </description>
        ///             </item>
        ///             <item>
        ///                 <description>
        ///                     Every transition has a TraceAction added to it for the same reason.
        ///                 </description>
        ///             </item>
        ///         </list>
        ///     </para>
        /// </remarks>
        public static Workflow Create(string id, IWorkflowMessageQueue workflowMessageQueue)
        {
            var dataCatalogItemRepositoryFactory = new DataCatalogItemRepositoryFactory();

            var workflow = new Workflow(
                id,
                "Data Catalog item workflow",
                "Controls the lifecycle of a data catalog item");

            WorkflowState initializing = workflow.CreateState(displayName: "Waiting for initialization");
            WorkflowState waitingForDocumentation = workflow.CreateState(displayName: "Waiting for documentation");
            WorkflowState published = workflow.CreateState(displayName: "Published");
            WorkflowState deleted = workflow.CreateState(displayName: "Deleted");
            WorkflowState deprecated = workflow.CreateState(displayName: "Deprecated");

            workflow.SetInitialState(initializing);

            initializing.EntryConditions.Add(
                new ContextItemsPresentCondition { RequiredContextItems = new[] { "Identifier", "Type" } });
            initializing.AddTraceActionForEntry();
            initializing.EntryActions.Add(new SendCreateCatalogItemTriggerAction(workflowMessageQueue));
            initializing.AddTraceActionForExit();

            waitingForDocumentation.AddTraceActionForEntry();
            waitingForDocumentation.AddTraceActionForExit();
            waitingForDocumentation.ExitConditions.Add(
                new ContextItemsPresentCondition { RequiredContextItems = new[] { "AllowWaitingForDocumentationExit" } });

            published.AddTraceActionForEntry();
            published.AddTraceActionForExit();
            published.EntryConditions.Add(
                new ContextItemsPresentCondition { RequiredContextItems = new[] { "AllowPublishedEntry" } });

            deleted.AddTraceActionForEntry();
            deleted.AddTraceActionForExit();

            deprecated.AddTraceActionForEntry();
            deprecated.AddTraceActionForExit();

            WorkflowTransition createCatalogItemTransition =
                initializing.CreateTransition(waitingForDocumentation, displayName: "Create catalog item");
            createCatalogItemTransition.Conditions.Add(
                new TriggerContentTypeCondition
                {
                    TriggerContentType = CreateCatalogItemTrigger.RegisteredContentType,
                });
            createCatalogItemTransition.Conditions.Add(new CatalogItemIdCondition());
            createCatalogItemTransition.Actions.Add(new CreateCatalogItemAction(dataCatalogItemRepositoryFactory));
            createCatalogItemTransition.AddTraceAction();

            WorkflowTransition waitingForDocumentationEditTransition =
                waitingForDocumentation.CreateTransition(waitingForDocumentation, displayName: "Edit");
            waitingForDocumentationEditTransition.Conditions.Add(
                new TriggerContentTypeCondition { TriggerContentType = EditCatalogItemTrigger.RegisteredContentType });
            waitingForDocumentationEditTransition.Conditions.Add(new CatalogItemIdCondition());
            waitingForDocumentationEditTransition.Actions.Add(
                new ApplyCatalogItemPatchAction(dataCatalogItemRepositoryFactory));
            waitingForDocumentationEditTransition.AddTraceAction();

            WorkflowTransition waitingForDocumentationPublishTransition =
                waitingForDocumentation.CreateTransition(published, displayName: "Publish");
            waitingForDocumentationPublishTransition.Conditions.Add(
                new TriggerContentTypeCondition
                {
                    TriggerContentType = PublishCatalogItemTrigger.RegisteredContentType,
                });
            waitingForDocumentationPublishTransition.Conditions.Add(new CatalogItemIdCondition());
            waitingForDocumentationPublishTransition.Conditions.Add(
                new CatalogItemCompleteCondition(dataCatalogItemRepositoryFactory));
            waitingForDocumentationPublishTransition.AddTraceAction();

            WorkflowTransition publishedEditCompleteTransition =
                published.CreateTransition(published, displayName: "Edit (complete)");
            publishedEditCompleteTransition.Conditions.Add(
                new TriggerContentTypeCondition { TriggerContentType = EditCatalogItemTrigger.RegisteredContentType });
            publishedEditCompleteTransition.Conditions.Add(new CatalogItemIdCondition());
            publishedEditCompleteTransition.Conditions.Add(
                new CatalogItemWillBeCompleteCondition(dataCatalogItemRepositoryFactory) { ExpectedResult = true });
            publishedEditCompleteTransition.Actions.Add(
                new ApplyCatalogItemPatchAction(dataCatalogItemRepositoryFactory));
            publishedEditCompleteTransition.AddTraceAction();

            WorkflowTransition publishedEditIncompleteTransition =
                published.CreateTransition(waitingForDocumentation, displayName: "Edit (incomplete)");
            publishedEditIncompleteTransition.Conditions.Add(
                new TriggerContentTypeCondition { TriggerContentType = EditCatalogItemTrigger.RegisteredContentType });
            publishedEditIncompleteTransition.Conditions.Add(new CatalogItemIdCondition());
            publishedEditIncompleteTransition.Conditions.Add(
                new CatalogItemWillBeCompleteCondition(dataCatalogItemRepositoryFactory) { ExpectedResult = false });
            publishedEditIncompleteTransition.Actions.Add(
                new ApplyCatalogItemPatchAction(dataCatalogItemRepositoryFactory));
            publishedEditIncompleteTransition.AddTraceAction();

            WorkflowTransition publishedDeleteTransition = published.CreateTransition(deleted, displayName: "Delete");
            publishedDeleteTransition.Conditions.Add(
                new TriggerContentTypeCondition
                {
                    TriggerContentType = DeleteCatalogItemTrigger.RegisteredContentType,
                });
            publishedDeleteTransition.Conditions.Add(new CatalogItemIdCondition());
            publishedDeleteTransition.AddTraceAction();

            WorkflowTransition publishedDeprecateTransition = published.CreateTransition(deprecated, displayName: "Deprecate");
            publishedDeprecateTransition.Conditions.Add(
                new TriggerContentTypeCondition
                {
                    TriggerContentType = DeprecateCatalogItemTrigger.RegisteredContentType,
                });
            publishedDeprecateTransition.Conditions.Add(new CatalogItemIdCondition());
            publishedDeprecateTransition.AddTraceAction();

            return workflow;
        }
    }
}