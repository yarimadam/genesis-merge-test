using CoreType.Attributes;
using CoreType.Types;

namespace CoreType.DBModels
{
    [SoftDelete(nameof(Status), (int) Types.Status.Deleted)]
    public partial class CoreParameters : TenantInfo
    {
    }
}