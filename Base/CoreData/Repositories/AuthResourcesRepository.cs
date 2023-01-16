using System.Linq;
using System.Threading.Tasks;
using CoreData.CacheManager;
using CoreData.Common;
using CoreData.Infrastructure;
using CoreData.Validators;
using CoreType.DBModels;
using CoreType.Types;
using FluentValidation;

namespace CoreData.Repositories
{
    public class AuthResourcesRepository : GenericGenesisRepository<AuthResources, int, AuthResourcesValidator>
    {
        public AuthResourcesRepository()
        {
        }

        public AuthResourcesRepository(GenesisContextBase context) : base(context)
        {
        }

        public override async Task<PaginationWrapper<AuthResources>> ListAsync(RequestWithPagination<AuthResources> request)
        {
            return await ListAsQueryable(request)
                .ThenBy(x => x.ResourceCode)
                .ToPaginatedListAsync(request);
        }

        public override async Task<AuthResources> SaveAsync(AuthResources entity, IValidator<AuthResources> validator, bool ignoreValidation = false)
        {
            AuthResources oldRecord = null;
            if (entity.ResourceId > 0)
                oldRecord = await GetAsync(entity, true);

            await base.SaveAsync(entity, validator, ignoreValidation);

            // Clear cached claims if resource is no longer active. 
            if (oldRecord != null
                && oldRecord.Status == (int) Status.Active
                && entity.Status != (int) Status.Active)
                await DistributedCache.ClearAllClaimsAsync();

            return entity;
        }
    }
}