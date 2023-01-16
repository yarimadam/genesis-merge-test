using CoreType.Attributes;

namespace CoreType
{
    public class IdentityUserInput
    {
        public string CustomClientId { get; set; }

        [HashedLogging]
        public string CustomSecretKey { get; set; }

        public string Email { get; set; }

        [HashedLogging]
        public string Password { get; set; }
    }

    public class IdentityUserTokenInput : IdentityUserInput
    {
        [HashedLogging]
        public string Token { get; set; }

        [IgnoreLogging]
        public string PrivateKey { get; set; }
    }
}