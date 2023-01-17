using System;
using System.Collections.Generic;

#nullable disable

namespace CoreType.DBModels
{
    public partial class CoreUser
    {
        public CoreUser()
        {
            AuthUserRights = new HashSet<AuthUserRight>();
            CoreDepartments = new HashSet<CoreDepartment>();
        }

        public int UserId { get; set; }
        public int? DepartmentId { get; set; }
        public int? RelationType { get; set; }
        public int? RelatedUserId { get; set; }
        public string IbanNumber { get; set; }
        public int? CountyId { get; set; }
        public int? CityId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int? CompanyId { get; set; }
        public string Email { get; set; }
        public int? Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public string RegistrationNumber { get; set; }
        public string IdentificationNo { get; set; }
        public string Password { get; set; }
        public string TempPassword { get; set; }
        public DateTime? WorkStartTime { get; set; }
        public DateTime? WorkEndTime { get; set; }
        public int? Status { get; set; }
        public string Address { get; set; }
        public int? MaritalStatus { get; set; }
        public string PhoneNumber { get; set; }
        public int? UserTitle { get; set; }
        public int? GeneralUser { get; set; }
        public DateTime? MarriageDate { get; set; }
        public short? IsAuthorized { get; set; }
        public int? RoleId { get; set; }
        public short? IdentityType { get; set; }
        public string ForgotPasswordKey { get; set; }
        public DateTime? ForgotPasswordExpiration { get; set; }
        public bool ShouldChangePassword { get; set; }
        public string VerificationKey { get; set; }
        public DateTime? VerificationKeyExpiration { get; set; }
        public int TenantId { get; set; }
        public int? CreatedUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? UpdatedUserId { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public virtual ICollection<AuthUserRight> AuthUserRights { get; set; }
        public virtual ICollection<CoreDepartment> CoreDepartments { get; set; }
    }
}
