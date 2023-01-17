using System;
using System.Collections.Generic;

#nullable disable

namespace CoreType.DBModels
{
    public partial class AuthAction
    {
        public AuthAction()
        {
            AuthUserRights = new HashSet<AuthUserRight>();
        }

        public int ActionId { get; set; }
        public int ResourceId { get; set; }
        public int ActionType { get; set; }
        public int OrderIndex { get; set; }
        public int Status { get; set; }
        public int TenantId { get; set; }
        public int? CreatedUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual AuthResource Resource { get; set; }
        public virtual ICollection<AuthUserRight> AuthUserRights { get; set; }
    }
}
