using CoreType.Attributes;

namespace CoreType.Models
{
    public class ChangePassword
    {
        public string Email { get; set; }

        [HashedLogging]
        public string CurrentPassword { get; set; }

        [HashedLogging]
        public string NewPassword { get; set; }
    }
}