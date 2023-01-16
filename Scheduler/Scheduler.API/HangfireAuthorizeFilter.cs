using Hangfire.Dashboard;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;

namespace Scheduler.API
{
    [Authorize]
    public class HangfireAuthorizeFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            var isAuthenticated = httpContext.User.Identity.IsAuthenticated;

            if (!isAuthenticated)
                httpContext.ChallengeAsync(OpenIdConnectDefaults.AuthenticationScheme).GetAwaiter().GetResult();

            return true;
        }
    }
}