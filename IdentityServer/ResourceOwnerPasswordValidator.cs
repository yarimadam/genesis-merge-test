using System.Linq;
using System.Threading.Tasks;
using CoreData.CacheManager;
using CoreType.DBModels;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using CoreHelper = CoreData.Common.Helper;

namespace IdentityServer
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            int userId;
            using (var dbContext = CoreHelper.GetGenesisContext())
            {
                userId = dbContext.Set<CoreUsers>()
                    .Where(x => x.Email == context.UserName && x.Password == CoreHelper.GetHashedString(context.Password, true))
                    .Select(x => x.UserId)
                    .FirstOrDefault();
            }

            if (userId <= 0)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, DistributedCache.GetParam("INCORRECT_USERNAME_OR_PASSWORD"));
                return Task.FromResult(0);
            }

            context.Result = new GrantValidationResult(userId.ToString(), OidcConstants.GrantTypes.Password);
            return Task.FromResult(0);
        }
    }
}