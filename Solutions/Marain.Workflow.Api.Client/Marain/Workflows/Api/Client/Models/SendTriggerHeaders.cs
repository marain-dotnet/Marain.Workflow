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
    /// Defines headers for sendTrigger operation.
    /// </summary>
    public partial class SendTriggerHeaders
    {
        /// <summary>
        /// Initializes a new instance of the SendTriggerHeaders class.
        /// </summary>
        public SendTriggerHeaders()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the SendTriggerHeaders class.
        /// </summary>
        /// <param name="location">A link to the endpoint in the Operations
        /// Status API for this operation</param>
        public SendTriggerHeaders(string location = default(string))
        {
            Location = location;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets a link to the endpoint in the Operations Status API
        /// for this operation
        /// </summary>
        [JsonProperty(PropertyName = "Location")]
        public string Location { get; set; }

    }
}
