// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Marain.Claims.Client.Models
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class ResourceAccessRuleSetWithGetExample : ResourceAccessRuleSet
    {
        /// <summary>
        /// Initializes a new instance of the
        /// ResourceAccessRuleSetWithGetExample class.
        /// </summary>
        public ResourceAccessRuleSetWithGetExample()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// ResourceAccessRuleSetWithGetExample class.
        /// </summary>
        public ResourceAccessRuleSetWithGetExample(string id, string eTag = default(string), string displayName = default(string), IList<ResourceAccessRule> rules = default(IList<ResourceAccessRule>))
            : base(id, eTag, displayName, rules)
        {
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Microsoft.Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public override void Validate()
        {
            base.Validate();
        }
    }
}
