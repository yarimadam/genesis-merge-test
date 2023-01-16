using System.Collections.Generic;
using CoreType.DBModels;

namespace CoreType.Types
{
    public class AuthorizationClaims
    {
        public List<AuthResources> AuthResources { get; set; } = new List<AuthResources>();
    }

    public class UserAuthorizationClaims
    {
        public int UserId { get; set; }
        public int? AuthTemplateId { get; set; }
        public AuthorizationClaims AuthorizationClaims { get; set; } = new AuthorizationClaims();
    }
}