// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Marain.Claims.Client.Models
{
    using System.Linq;

    public partial class ClaimPermissionsBatchResponseItemWithExample : ClaimPermissionsBatchResponseItem
    {
        /// <summary>
        /// Initializes a new instance of the
        /// ClaimPermissionsBatchResponseItemWithExample class.
        /// </summary>
        public ClaimPermissionsBatchResponseItemWithExample()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// ClaimPermissionsBatchResponseItemWithExample class.
        /// </summary>
        /// <param name="permission">Possible values include: 'allow',
        /// 'deny'</param>
        public ClaimPermissionsBatchResponseItemWithExample(string claimPermissionsId = default(string), string resourceUri = default(string), string resourceAccessType = default(string), int? responseCode = default(int?), string permission = default(string))
            : base(claimPermissionsId, resourceUri, resourceAccessType, responseCode, permission)
        {
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

    }
}
