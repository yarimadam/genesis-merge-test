using System.ComponentModel.DataAnnotations.Schema;
using CoreType.Types;

namespace CoreType.DBModels
{
    public partial class Tenant : GenesisBaseContract
    {
        [NotMapped]
        public string ParentTenantName { get; set; }
    }
}