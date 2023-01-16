using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoreData.CacheManager;
using CoreSvc.Common;
using CoreType.Types;
using IdentityModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using Helper = CoreData.Common.Helper;

namespace CoreSvc.Filters
{
    public class ClaimRequirementAttribute : ActionFilterAttribute
    {
        private IList<string> _resourceCodes;
        private readonly ActionType _actionType;

        public ClaimRequirementAttribute(ActionType actionType, params string[] extraResourceCodes)
        {
            _resourceCodes = extraResourceCodes.ToList();
            _actionType = actionType;
        }

        public ClaimRequirementAttribute(string extraResourceCode, ActionType actionType)
        {
            _resourceCodes = new[] { extraResourceCode };
            _actionType = actionType;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                int? sessionUserId = null;
                if (context.Controller is BaseController controller)
                {
                    sessionUserId = controller.Session?.CurrentUser?.UserId;
                    if (controller.ResourceCodes != null)
                        _resourceCodes = controller.ResourceCodes.Union(_resourceCodes).ToList();
                }

                if (_resourceCodes.Any())
                {
                    bool hasAuthorizationClaim = false;

                    var userId = sessionUserId.HasValue
                        ? sessionUserId.Value.ToString()
                        : context.HttpContext.User.FindFirstValue(JwtClaimTypes.Subject);

                    if (userId != null)
                    {
                        var claims = await DistributedCache.GetClaimsAsync(Convert.ToInt32(userId));
                        if (claims != null)
                            hasAuthorizationClaim = Helper.HasAuthorizationClaim(claims, _resourceCodes, _actionType);
                        else
                        {
                            Log.Warning("Claims does not exists for {userId} !", userId);
                            context.Result = new UnauthorizedResult();
                            return;
                        }
                    }
                    else
                        Log.Warning("User does not exists !");

                    if (!hasAuthorizationClaim)
                    {
                        Log.Warning("User with Id({userId}) has no access !", userId);
                        context.Result = new ForbidResult();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "ClaimRequirementAttribute.OnActionExecuting");
            }

            await next();
        }
    }
}