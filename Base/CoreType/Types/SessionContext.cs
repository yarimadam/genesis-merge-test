using System;
using CoreType.Models;

namespace CoreType.Types
{
    public class SessionContext
    {
        public LoggedInUser CurrentUser { get; set; } = new LoggedInUser();
        public AuthorizationClaims AuthorizationClaims { get; set; } = new AuthorizationClaims();
        public string SessionGuid { get; set; }
        public string Token { get; set; }
        public DateTime? TokenExpires { get; set; }
        public DateTime? LastLogin { get; set; }
        public bool IsAuthenticated { get; set; }
        public string PreferredLocale { get; set; }
        public string RequestId { get; set; }
    }
}