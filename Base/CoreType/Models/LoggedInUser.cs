using System.ComponentModel.DataAnnotations;
using CoreType.Attributes;
using CoreType.Types;

namespace CoreType.Models
{
    public class LoggedInUser : SubTenantInfo
    {
        [Key]
        public int UserId { get; set; }

        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }

        [HashedLogging]
        public string Password { get; set; }

        [HashedLogging]
        public string ForgotPasswordKey { get; set; }

        public int? RoleId { get; set; }
        public string RoleName { get; set; }

        public bool ShouldChangePassword { get; set; }
    }
}