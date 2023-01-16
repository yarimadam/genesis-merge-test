using System;
using CoreType.Attributes;
using CoreType.Types;

namespace CoreType.DBModels
{
    public partial class SampleEmployee
    {
        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; }

        public string EmployeeSurname { get; set; }

        public int CompanyId { get; set; }

        public int? EmployeeTitle { get; set; }

        public string Email { get; set; }

        [HashedLogging]
        public string Password { get; set; }

        public short? Gender { get; set; }

        [MaskedLogging]
        public string PhoneNumber { get; set; }

        [MaskedLogging]
        public string IbanNumber { get; set; }

        public string TaxNumber { get; set; }

        //[Column("countriesServed")]
        //public List<int> CountriesServed { get; set; } = new List<int>();

        public int? CityId { get; set; }

        public int? CountyId { get; set; }

        public DateTime WorkStartDate { get; set; }

        [IgnoreLogging]
        public Decimal? Salary { get; set; }

        public string Note { get; set; }

        public FileContent Picture { get; set; }

        public int Status { get; set; }
    }
}