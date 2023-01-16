using IdentityModel;
using Microsoft.AspNetCore.SignalR;

namespace CoreSvc.Common
{
    public class NameUserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst(JwtClaimTypes.Id)?.Value;
        }
    }
}