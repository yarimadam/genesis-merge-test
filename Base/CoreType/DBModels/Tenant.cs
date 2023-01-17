using System;
using System.Collections.Generic;

#nullable disable

namespace CoreType.DBModels
{
    public partial class Tenant
    {
        public Tenant()
        {
            InverseParentTenant = new HashSet<Tenant>();
        }

        public int TenantId { get; set; }
        public int? ParentTenantId { get; set; }
        public string TenantName { get; set; }
        public int TenantType { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string TaxOffice { get; set; }
        public string TaxNumber { get; set; }
        public int? CountryId { get; set; }
        public int? CityId { get; set; }
        public string Note { get; set; }
        public string Website { get; set; }
        public int Status { get; set; }
        public bool IsDefault { get; set; }
        public int? CreatedUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual Tenant ParentTenant { get; set; }
        public virtual ICollection<Tenant> InverseParentTenant { get; set; }
    }
}
