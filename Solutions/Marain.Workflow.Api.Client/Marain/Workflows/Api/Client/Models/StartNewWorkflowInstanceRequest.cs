// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Marain.Workflows.Api.Client.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A request to create a new instance of a workflow
    /// </summary>
    public partial class StartNewWorkflowInstanceRequest
    {
        /// <summary>
        /// Initializes a new instance of the StartNewWorkflowInstanceRequest
        /// class.
        /// </summary>
        public StartNewWorkflowInstanceRequest()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the StartNewWorkflowInstanceRequest
        /// class.
        /// </summary>
        /// <param name="workflowId">Id of the workflow to start an instance
        /// of. This should match the Id of an existing workflow stored within
        /// the hosted workflow service.</param>
        /// <param name="requestId">Unique Id for this request. The Id will be
        /// used internally to trace the path of the request through the
        /// workflow engine and can be used to retrieve the status of the
        /// trigger. If omitted, a request Id will be generated by the
        /// server.</param>
        /// <param name="workflowInstanceId">Id to give the new workflow
        /// instance. If omitted, an Id will be generated.</param>
        /// <param name="context">Parameters for this trigger. This will be
        /// processed on the server as a list of key/value pairs - complex
        /// objects should not be used.</param>
        public StartNewWorkflowInstanceRequest(string workflowId, string requestId = default(string), string workflowInstanceId = default(string), IDictionary<string, string> context = default(IDictionary<string, string>))
        {
            RequestId = requestId;
            WorkflowId = workflowId;
            WorkflowInstanceId = workflowInstanceId;
            Context = context;
            CustomInit();
        }
        /// <summary>
        /// Static constructor for StartNewWorkflowInstanceRequest class.
        /// </summary>
        static StartNewWorkflowInstanceRequest()
        {
            ContentType = "application/vnd.marain.workflows.hosted.startworkflowinstancerequest";
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets unique Id for this request. The Id will be used
        /// internally to trace the path of the request through the workflow
        /// engine and can be used to retrieve the status of the trigger. If
        /// omitted, a request Id will be generated by the server.
        /// </summary>
        [JsonProperty(PropertyName = "requestId")]
        public string RequestId { get; set; }

        /// <summary>
        /// Gets or sets id of the workflow to start an instance of. This
        /// should match the Id of an existing workflow stored within the
        /// hosted workflow service.
        /// </summary>
        [JsonProperty(PropertyName = "workflowId")]
        public string WorkflowId { get; set; }

        /// <summary>
        /// Gets or sets id to give the new workflow instance. If omitted, an
        /// Id will be generated.
        /// </summary>
        [JsonProperty(PropertyName = "workflowInstanceId")]
        public string WorkflowInstanceId { get; set; }

        /// <summary>
        /// Gets or sets parameters for this trigger. This will be processed on
        /// the server as a list of key/value pairs - complex objects should
        /// not be used.
        /// </summary>
        [JsonProperty(PropertyName = "context")]
        public IDictionary<string, string> Context { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "contentType")]
        public static string ContentType { get; private set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (WorkflowId == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "WorkflowId");
            }
        }
    }
}
