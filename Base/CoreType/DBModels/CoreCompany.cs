using System.Collections.Generic;

namespace CoreType.DBModels
{
    public partial class CoreCompany
    {
        public CoreCompany()
        {
            CoreDepartment = new HashSet<CoreDepartment>();
        }

        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyLegalTitle { get; set; }
        public int? SectorId { get; set; }
        public int? NumberOfStaff { get; set; }
        public string TaxOffice { get; set; }
        public string TaxNumber { get; set; }
        public string BillingAddress { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPersonTitle { get; set; }
        public string ContactPersonTelephone { get; set; }
        public string ContactPersonEmail { get; set; }
        public int? CountryId { get; set; }
        public int? CityId { get; set; }
        public int? TownId { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string Note { get; set; }
        public int Status { get; set; }

        public virtual ICollection<CoreDepartment> CoreDepartment { get; set; }
    }
}