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
    /// Information about a failed operation
    /// </summary>
    public partial class ProblemDetails
    {
        /// <summary>
        /// Initializes a new instance of the ProblemDetails class.
        /// </summary>
        public ProblemDetails()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the ProblemDetails class.
        /// </summary>
        public ProblemDetails(int status, string detail, string title = default(string), string instance = default(string), string type = default(string), IList<object> validationErrors = default(IList<object>))
        {
            Status = status;
            Detail = detail;
            Title = title;
            Instance = instance;
            Type = type;
            ValidationErrors = validationErrors;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public int Status { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "detail")]
        public string Detail { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "instance")]
        public string Instance { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "validationErrors")]
        public IList<object> ValidationErrors { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (Detail == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "Detail");
            }
        }
    }
}
