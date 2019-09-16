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
        /// <param name="administratorRoleClaimValue">The value which, if
        /// present in a roles claim, will grant a principal full access to the
        /// Claims service API</param>
        public Body(string administratorRoleClaimValue)
        {
            AdministratorRoleClaimValue = administratorRoleClaimValue;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets the value which, if present in a roles claim, will
        /// grant a principal full access to the Claims service API
        /// </summary>
        [JsonProperty(PropertyName = "administratorRoleClaimValue")]
        public string AdministratorRoleClaimValue { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
            if (AdministratorRoleClaimValue == null)
            {
                throw new ValidationException(ValidationRules.CannotBeNull, "AdministratorRoleClaimValue");
            }
        }
    }
}