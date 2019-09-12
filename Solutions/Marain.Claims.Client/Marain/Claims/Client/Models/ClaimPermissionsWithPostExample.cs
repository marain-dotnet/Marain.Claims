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

    public partial class ClaimPermissionsWithPostExample : ClaimPermissions
    {
        /// <summary>
        /// Initializes a new instance of the ClaimPermissionsWithPostExample
        /// class.
        /// </summary>
        public ClaimPermissionsWithPostExample()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the ClaimPermissionsWithPostExample
        /// class.
        /// </summary>
        public ClaimPermissionsWithPostExample(string id, IList<ResourceAccessRule> resourceAccessRules = default(IList<ResourceAccessRule>), IList<ResourceAccessRuleSet> resourceAccessRuleSets = default(IList<ResourceAccessRuleSet>))
            : base(id, resourceAccessRules, resourceAccessRuleSets)
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
