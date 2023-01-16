using System;
using System.Collections.Generic;
using CoreType.Attributes;

namespace CoreType.DBModels
{
    public partial class CoreUsers
    {
        public CoreUsers()
        {
            AuthUserRights = new HashSet<AuthUserRights>();
            CoreDepartment = new HashSet<CoreDepartment>();
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

        [HashedLogging]
        public string Password { get; set; }

        [HashedLogging]
        public string TempPassword { get; set; }

        public DateTime? WorkStartTime { get; set; }
        public DateTime? WorkEndTime { get; set; }
        public int? Status { get; set; }

        [MaskedLogging(Regexes.AddressMaskingPattern)]
        public string Address { get; set; }

        public int? MaritalStatus { get; set; }
        public string PhoneNumber { get; set; }
        public int? UserTitle { get; set; }
        public int? GeneralUser { get; set; }
        public DateTime? MarriageDate { get; set; }
        public short? IsAuthorized { get; set; }
        public int? RoleId { get; set; }
        public short? IdentityType { get; set; }

        [HashedLogging]
        public string ForgotPasswordKey { get; set; }

        public DateTime? ForgotPasswordExpiration { get; set; }
        public bool ShouldChangePassword { get; set; }

        [HashedLogging]
        public string VerificationKey { get; set; }

        public DateTime? VerificationKeyExpiration { get; set; }

        public virtual ICollection<AuthUserRights> AuthUserRights { get; set; }
        public virtual ICollection<CoreDepartment> CoreDepartment { get; set; }
    }
}