// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Marain.Workflows.Api.Client.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    /// <summary>
    /// Defines headers for sendStartNewWorkflowInstanceRequest operation.
    /// </summary>
    public partial class SendStartNewWorkflowInstanceRequestHeaders
    {
        /// <summary>
        /// Initializes a new instance of the
        /// SendStartNewWorkflowInstanceRequestHeaders class.
        /// </summary>
        public SendStartNewWorkflowInstanceRequestHeaders()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// SendStartNewWorkflowInstanceRequestHeaders class.
        /// </summary>
        /// <param name="location">A link to endpoint in the Operations Status
        /// API for this operation</param>
        public SendStartNewWorkflowInstanceRequestHeaders(string location = default(string))
        {
            Location = location;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets a link to endpoint in the Operations Status API for
        /// this operation
        /// </summary>
        [JsonProperty(PropertyName = "Location")]
        public string Location { get; set; }

    }
}
