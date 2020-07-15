// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Marain.Claims.Client.Models
{
    using Microsoft.Rest;
    using Newtonsoft.Json;
    using System.Linq;

    public partial class Body
    {
        /// <summary>
        /// Initializes a new instance of the Body class.
        /// </summary>
        public Body()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the Body class.
        /// </summary>
        /// <param name="administratorPrincipalObjectId">The object of the
        /// principal of which to grant full access to the Claims service
        /// API</param>
        public Body(string administratorPrincipalObjectId)
        {
            AdministratorPrincipalObjectId = administratorPrincipalObjectId;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets the object of the principal of which to grant full
        /// access to the Claims service API
        /// </summary>
        [JsonProperty(PropertyName = "administratorPrincipalObjectId")]
        public string AdministratorPrincipalObjectId { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (AdministratorPrincipalObjectId == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "AdministratorPrincipalObjectId");
            }
        }
    }
}
