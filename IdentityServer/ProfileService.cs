using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoreData.CacheManager;
using CoreSvc.Services;
using CoreType.DBModels;
using CoreType.Types;
using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;

namespace IdentityServer
{
    public class ProfileService : IProfileService
    {
        private readonly AuthService _authService = new AuthService();
        private readonly UserService _userService = new UserService();
        private readonly TenantService _tenantService = new TenantService();
        private readonly AuthTemplatesService _authTemplatesService = new AuthTemplatesService();

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var userId = Convert.ToInt32(context.Subject.GetSubjectId());
            var user = await _userService.Repository.GetByIdAsync(userId);

            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var tenant = user.TenantId == 0
                ? new Tenant
                {
                    TenantId = 0,
                    TenantType = (int) TenantType.SystemOwner,
                }
                : await _tenantService.Repository.GetByIdAsync(user.TenantId);

            if (tenant == null)
                throw new ArgumentNullException(nameof(tenant));

            if (tenant.TenantType == 0)
                throw new ArgumentNullException(nameof(tenant.TenantType));

            var subTenants = _tenantService.Repository.ListSubTenants(user.TenantId);

            if (subTenants == null)
                throw new ArgumentNullException(nameof(subTenants));

            var roleName = user.RoleId != null ? _authTemplatesService.Repository.GetTemplateName(user.RoleId.Value) : null;

            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Subject, user.UserId.ToString()),
                new Claim(JwtClaimTypes.Id, user.UserId.ToString()),
                new Claim(JwtClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(JwtClaimTypes.Name, user.Name ?? string.Empty),
                new Claim(JwtClaimTypes.FamilyName, user.Surname ?? string.Empty),
                new Claim(CustomJwtClaimTypes.RoleId, user.RoleId?.ToString() ?? string.Empty),
                new Claim(CustomJwtClaimTypes.RoleName, roleName ?? string.Empty),
                new Claim(CustomJwtClaimTypes.TenantId, user.TenantId.ToString()),
                new Claim(CustomJwtClaimTypes.TenantName, tenant.TenantName ?? string.Empty),
                new Claim(CustomJwtClaimTypes.TenantType, tenant.TenantType.ToString()),
                new Claim(CustomJwtClaimTypes.SubTenantIds, string.Join(",", subTenants.Select(x => x.TenantId))),
                new Claim(CustomJwtClaimTypes.ShouldChangePassword, user.ShouldChangePassword.ToString()),
                new Claim(CustomJwtClaimTypes.ParentTenantId, tenant.ParentTenantId.ToString())
            };

            context.IssuedClaims = claims;

            var authenticationType = context.Subject.Identity.AuthenticationType;
            if (authenticationType == OidcConstants.GrantTypes.Password || authenticationType == "UserInfo")
                await DistributedCache.SetClaimsAsync(user.UserId, _authService.Repository.GetAuthorizationClaims(user.UserId));
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var subjectId = context.Subject.GetSubjectId();
            if (subjectId.Equals("invalid_request"))
            {
                context.IsActive = false;
            }
            else
            {
                var userId = Convert.ToInt32(subjectId);
                var user = await _userService.Repository.GetByIdAsync(userId);
                context.IsActive = (user != null); //&& user.Active;
            }
        }
    }
}