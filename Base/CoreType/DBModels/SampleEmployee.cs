using System;
using System.Collections.Generic;

#nullable disable

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
        public string Password { get; set; }
        public short? Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string IbanNumber { get; set; }
        public string TaxNumber { get; set; }
        public int? CityId { get; set; }
        public int? CountyId { get; set; }
        public DateTime WorkStartDate { get; set; }
        public decimal? Salary { get; set; }
        public string Note { get; set; }
        public string Picture { get; set; }
        public int Status { get; set; }
    }
}
