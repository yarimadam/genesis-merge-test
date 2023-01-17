using System;
using System.Collections.Generic;

#nullable disable

namespace CoreType.DBModels
{
    public partial class AuthUserRight
    {
        public int RightId { get; set; }
        public int ActionId { get; set; }
        public int UserId { get; set; }
        public int RecordType { get; set; }
        public int? GroupActionId { get; set; }
        public int Status { get; set; }
        public int TenantId { get; set; }
        public int? CreatedUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual AuthAction Action { get; set; }
        public virtual CoreUser User { get; set; }
    }
}
