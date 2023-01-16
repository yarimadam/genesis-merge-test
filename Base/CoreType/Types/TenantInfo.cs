using System.Collections.Generic;

namespace CoreType.Types
{
    public interface ITenantInfo
    {
        int TenantId { set; get; }
    }

    public abstract class TenantInfo : ITenantInfo
    {
        public int TenantId { get; set; }
    }

    public abstract class SubTenantInfo : TenantInfo
    {
        public string TenantName { get; set; }
        public int TenantType { get; set; }
        public List<int> SubTenantIds { get; set; } = new List<int>();
        public int? XTenantId { get; set; }
        public int? XTenantType { get; set; }
        public List<int> XSubTenantIds { get; set; } = new List<int>();
        public int? ParentTenantId { get; set; }
        public int? XParentTenantId { get; set; }
    }
}