using System.ComponentModel.DataAnnotations.Schema;
using CoreType.Attributes;
using CoreType.Types;

namespace CoreType.DBModels
{
    [SoftDelete(nameof(Status), (int) Types.Status.Deleted)]
    public partial class CoreUsers : GenesisBaseContract
    {
        [NotMapped]
        public string TenantName { get; set; }

        [NotMapped]
        public string RoleName { get; set; }
    }
}