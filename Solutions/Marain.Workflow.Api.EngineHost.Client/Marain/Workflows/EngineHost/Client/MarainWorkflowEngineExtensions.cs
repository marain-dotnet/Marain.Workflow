// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Marain.Workflows.EngineHost.Client
{
    using Models;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Extension methods for MarainWorkflowEngine.
    /// </summary>
    public static partial class MarainWorkflowEngineExtensions
    {
            /// <summary>
            /// Start a new instance of a workflow
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='tenantId'>
            /// The tenant within which the request should operate
            /// </param>
            /// <param name='body'>
            /// </param>
            public static void StartWorkflowInstance(this IMarainWorkflowEngine operations, string tenantId, StartWorkflowRequest body)
            {
                operations.StartWorkflowInstanceAsync(tenantId, body).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Start a new instance of a workflow
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='tenantId'>
            /// The tenant within which the request should operate
            /// </param>
            /// <param name='body'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task StartWorkflowInstanceAsync(this IMarainWorkflowEngine operations, string tenantId, StartWorkflowRequest body, CancellationToken cancellationToken = default(CancellationToken))
            {
                (await operations.StartWorkflowInstanceWithHttpMessagesAsync(tenantId, body, null, cancellationToken).ConfigureAwait(false)).Dispose();
            }

            /// <summary>
            /// Dispatch a trigger for processing by a specific workflow instance.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='tenantId'>
            /// The tenant within which the request should operate
            /// </param>
            /// <param name='workflowInstanceId'>
            /// The Id of the workflow instance to apply the trigger to
            /// </param>
            /// <param name='body'>
            /// New trigger to be processed by the engine
            /// </param>
            public static void SendTrigger(this IMarainWorkflowEngine operations, string tenantId, string workflowInstanceId, Trigger body)
            {
                operations.SendTriggerAsync(tenantId, workflowInstanceId, body).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Dispatch a trigger for processing by a specific workflow instance.
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='tenantId'>
            /// The tenant within which the request should operate
            /// </param>
            /// <param name='workflowInstanceId'>
            /// The Id of the workflow instance to apply the trigger to
            /// </param>
            /// <param name='body'>
            /// New trigger to be processed by the engine
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task SendTriggerAsync(this IMarainWorkflowEngine operations, string tenantId, string workflowInstanceId, Trigger body, CancellationToken cancellationToken = default(CancellationToken))
            {
                (await operations.SendTriggerWithHttpMessagesAsync(tenantId, workflowInstanceId, body, null, cancellationToken).ConfigureAwait(false)).Dispose();
            }

            /// <summary>
            /// Create a workflow definition
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='tenantId'>
            /// The tenant within which the request should operate
            /// </param>
            /// <param name='body'>
            /// </param>
            public static void CreateWorkflow(this IMarainWorkflowEngine operations, string tenantId, Workflow body)
            {
                operations.CreateWorkflowAsync(tenantId, body).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Create a workflow definition
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='tenantId'>
            /// The tenant within which the request should operate
            /// </param>
            /// <param name='body'>
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task CreateWorkflowAsync(this IMarainWorkflowEngine operations, string tenantId, Workflow body, CancellationToken cancellationToken = default(CancellationToken))
            {
                (await operations.CreateWorkflowWithHttpMessagesAsync(tenantId, body, null, cancellationToken).ConfigureAwait(false)).Dispose();
            }

            /// <summary>
            /// Get a workflow
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='tenantId'>
            /// The tenant within which the request should operate
            /// </param>
            /// <param name='workflowId'>
            /// The Id of the workflow to retrieve
            /// </param>
            public static Workflow GetWorkflow(this IMarainWorkflowEngine operations, string tenantId, string workflowId)
            {
                return operations.GetWorkflowAsync(tenantId, workflowId).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Get a workflow
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='tenantId'>
            /// The tenant within which the request should operate
            /// </param>
            /// <param name='workflowId'>
            /// The Id of the workflow to retrieve
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<Workflow> GetWorkflowAsync(this IMarainWorkflowEngine operations, string tenantId, string workflowId, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetWorkflowWithHttpMessagesAsync(tenantId, workflowId, null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

            /// <summary>
            /// Update a workflow definition
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='tenantId'>
            /// The tenant within which the request should operate
            /// </param>
            /// <param name='workflowId'>
            /// The Id of the workflow to retrieve
            /// </param>
            /// <param name='body'>
            /// </param>
            /// <param name='ifMatch'>
            /// The ETag of the last known version.
            /// </param>
            public static void UpdateWorkflow(this IMarainWorkflowEngine operations, string tenantId, string workflowId, Workflow body, string ifMatch = default(string))
            {
                operations.UpdateWorkflowAsync(tenantId, workflowId, body, ifMatch).GetAwaiter().GetResult();
            }

            /// <summary>
            /// Update a workflow definition
            /// </summary>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='tenantId'>
            /// The tenant within which the request should operate
            /// </param>
            /// <param name='workflowId'>
            /// The Id of the workflow to retrieve
            /// </param>
            /// <param name='body'>
            /// </param>
            /// <param name='ifMatch'>
            /// The ETag of the last known version.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task UpdateWorkflowAsync(this IMarainWorkflowEngine operations, string tenantId, string workflowId, Workflow body, string ifMatch = default(string), CancellationToken cancellationToken = default(CancellationToken))
            {
                (await operations.UpdateWorkflowWithHttpMessagesAsync(tenantId, workflowId, body, ifMatch, null, cancellationToken).ConfigureAwait(false)).Dispose();
            }

            /// <summary>
            /// View swagger definition for this API
            /// </summary>
            /// <remarks>
            /// View swagger definition for this API
            /// </remarks>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            public static object GetSwagger(this IMarainWorkflowEngine operations)
            {
                return operations.GetSwaggerAsync().GetAwaiter().GetResult();
            }

            /// <summary>
            /// View swagger definition for this API
            /// </summary>
            /// <remarks>
            /// View swagger definition for this API
            /// </remarks>
            /// <param name='operations'>
            /// The operations group for this extension method.
            /// </param>
            /// <param name='cancellationToken'>
            /// The cancellation token.
            /// </param>
            public static async Task<object> GetSwaggerAsync(this IMarainWorkflowEngine operations, CancellationToken cancellationToken = default(CancellationToken))
            {
                using (var _result = await operations.GetSwaggerWithHttpMessagesAsync(null, cancellationToken).ConfigureAwait(false))
                {
                    return _result.Body;
                }
            }

    }
}
