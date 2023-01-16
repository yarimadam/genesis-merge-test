using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreData.Repositories;
using CoreType.Models;
using CoreType.Types;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace CoreData.Common
{
    public class SessionAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        //TODO Define a system tenant like genesis, if there is no session that will be used by default.
        public SessionContext Session => GetLoggedInUserSession().GetAwaiter().GetResult();

        #region Static Access

        private static SessionContext _lastSession;

        public static SessionContext GetSession() => ServiceLocator.GetSession() ?? _lastSession;

        #endregion

        private async Task<SessionContext> GetLoggedInUserSession()
        {
            SessionContext sessionContext = null;
            var httpContext = _httpContextAccessor?.HttpContext;
            var user = httpContext?.User;

            if (user != null)
            {
                var userId = Convert.ToInt32(user.FindFirst(JwtClaimTypes.Subject)?.Value);

                // var sessionLockKey = DistributedCache.CreateSessionKey(userId) + "_lock";
                // var token = Guid.NewGuid();
                //
                // if (DistributedCache.LockTake(sessionLockKey, token.ToString()))
                // {
                //     try
                //     {
                //         var cachedSession = DistributedCache.GetSession(userId);
                //
                //         if (cachedSession != null)
                //             return cachedSession;

                var userMail = user.FindFirst(JwtClaimTypes.Email)?.Value;
                var name = user.FindFirst(JwtClaimTypes.Name)?.Value;
                var surname = user.FindFirst(JwtClaimTypes.FamilyName)?.Value;
                var roleId = user.FindFirst(CustomJwtClaimTypes.RoleId)?.Value;
                var roleName = user.FindFirst(CustomJwtClaimTypes.RoleName)?.Value;
                var tenantId = user.FindFirst(CustomJwtClaimTypes.TenantId)?.Value;
                var tenantName = user.FindFirst(CustomJwtClaimTypes.TenantName)?.Value;
                var tenantType = user.FindFirst(CustomJwtClaimTypes.TenantType)?.Value;
                var subTenantIds = user.FindFirst(CustomJwtClaimTypes.SubTenantIds)?.Value;
                var shouldChangePassword = user.FindFirst(CustomJwtClaimTypes.ShouldChangePassword)?.Value;
                var parentTenantId = user.FindFirst(CustomJwtClaimTypes.ParentTenantId)?.Value;

                var currentAuthenticatedUser = new LoggedInUser
                {
                    UserId = Convert.ToInt32(userId),
                    Name = name,
                    Surname = surname,
                    Email = userMail,
                    RoleId = !string.IsNullOrEmpty(roleId) ? Convert.ToInt32(roleId) : (int?) null,
                    RoleName = roleName,
                    TenantId = Convert.ToInt32(tenantId),
                    TenantName = tenantName,
                    TenantType = Convert.ToInt32(tenantType),
                    SubTenantIds = !string.IsNullOrEmpty(subTenantIds)
                        ? subTenantIds.Split(",").Select(x => Convert.ToInt32(x)).ToList()
                        : new List<int>(),
                    ShouldChangePassword = !string.IsNullOrEmpty(shouldChangePassword) && Convert.ToBoolean(shouldChangePassword),
                    ParentTenantId = !string.IsNullOrEmpty(parentTenantId) ? (int?) Convert.ToInt32(parentTenantId) : null,
                };

                var token = user.Identity.IsAuthenticated ? await httpContext.GetTokenAsync(Constants.ACCESS_TOKEN_NAME) : null;

                sessionContext = new SessionContext
                {
                    IsAuthenticated = user.Identity.IsAuthenticated,
                    CurrentUser = currentAuthenticatedUser,
                    Token = token
                };

                var tempLocale = httpContext.Request.Headers
                    .Where(x => x.Key.Equals("Accept-Language") || x.Key.Equals("X-Accept-Language"))
                    .OrderByDescending(x => x.Key)
                    .Select(x => x.Value.ToString())
                    .FirstOrDefault();

                if (string.IsNullOrEmpty(tempLocale) || tempLocale.Equals("*"))
                    sessionContext.PreferredLocale = Constants.DEFAULT_LANGUAGE;
                else
                    sessionContext.PreferredLocale = tempLocale.Substring(0, 2).ToUpperInvariant();

                var selectedTenantIdStr = httpContext.Request.Headers
                    .Where(x => x.Key.Equals("X-TenantId"))
                    .Select(x => x.Value.ToString())
                    .FirstOrDefault();

                sessionContext.RequestId = httpContext.TraceIdentifier;

                if (!string.IsNullOrEmpty(selectedTenantIdStr))
                {
                    int selectedTenantId = Convert.ToInt32(selectedTenantIdStr);
                    if (sessionContext.CurrentUser.TenantId != selectedTenantId
                        && (sessionContext.CurrentUser.TenantType == (int) TenantType.SystemOwner
                            || currentAuthenticatedUser.SubTenantIds.Contains(selectedTenantId)))
                    {
                        var tenantRepository = new TenantRepository(sessionContext);

                        var tenant = await tenantRepository.GetByIdAsync(selectedTenantId);

                        if (tenant != null)
                        {
                            // TODO Get tenant and subtenants in one query, evaluate ListTenantAndSubTenants
                            var subTenants = tenantRepository
                                .ListSubTenants(selectedTenantId)
                                .Select(x => x.TenantId)
                                .ToList();

                            sessionContext.CurrentUser.XTenantId = selectedTenantId;
                            sessionContext.CurrentUser.XTenantType = tenant.TenantType;
                            sessionContext.CurrentUser.XSubTenantIds = subTenants;
                            sessionContext.CurrentUser.XParentTenantId = tenant.ParentTenantId;
                        }
                    }
                }

                //     DistributedCache.SetSession(userId, sessionContext);
                // }
                // finally
                // {
                //     DistributedCache.LockRelease(sessionLockKey, token.ToString());
                // }
                // }

                _lastSession = sessionContext;
            }

            return sessionContext;
        }
    }
}