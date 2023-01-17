using System;
using System.Collections.Generic;

#nullable disable

namespace CoreType.DBModels
{
    public partial class CoreDepartment
    {
        public int DepartmentId { get; set; }
        public int? CompanyId { get; set; }
        public string DepartmentName { get; set; }
        public int? ParentDepartmentId { get; set; }
        public int? DepHeadUserId { get; set; }
        public string Description { get; set; }
        public short Status { get; set; }
        public int TenantId { get; set; }
        public int? CreatedUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual CoreCompany Company { get; set; }
        public virtual CoreUser DepHeadUser { get; set; }
    }
}
