using System;
using System.Collections.Generic;

#nullable disable

namespace CoreType.DBModels
{
    public partial class AuthResource
    {
        public AuthResource()
        {
            AuthActions = new HashSet<AuthAction>();
        }

        public int ResourceId { get; set; }
        public string ParentResourceCode { get; set; }
        public string ResourceCode { get; set; }
        public string ResourceName { get; set; }
        public int ResourceType { get; set; }
        public int OrderIndex { get; set; }
        public int Status { get; set; }
        public string SeoTitle { get; set; }
        public string SeoDescription { get; set; }
        public string SeoKeywords { get; set; }
        public string TableName { get; set; }
        public int TenantId { get; set; }
        public int? CreatedUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual ICollection<AuthAction> AuthActions { get; set; }
    }
}
