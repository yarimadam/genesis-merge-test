using System;

namespace CoreType.Types
{
    public interface IBaseContract
    {
        int? CreatedUserId { get; set; }
        DateTime? CreatedDate { get; set; }
        int? UpdatedUserId { get; set; }
        DateTime? UpdatedDate { get; set; }
    }

    public abstract class GenesisBaseContract : TenantInfo, IBaseContract
    {
        public int? CreatedUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    // Can be used to extend all models from one place
    public abstract class BaseContract /*: TenantInfo, IBaseContract*/
    {
        // public int? CreatedUserId { get; set; }
        // public DateTime? CreatedDate { get; set; }
        // public int? UpdatedUserId { get; set; }
        // public DateTime? UpdatedDate { get; set; }
    }
}