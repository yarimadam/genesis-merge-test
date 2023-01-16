using System.Linq;
using System.Threading.Tasks;
using Admin.Data.Validators;
using CoreData.Common;
using CoreData.Infrastructure;
using CoreType.DBModels;
using CoreType.Types;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using CoreHelper = CoreData.Common.Helper;

namespace Admin.Data.Repositories
{
    public class SampleEmployeeRepository : GenericGenesisRepository<SampleEmployee, int, SampleEmployeeValidator>
    {
        public SampleEmployeeRepository()
        {
        }

        public SampleEmployeeRepository(GenesisContextBase context) : base(context)
        {
        }

        public override async Task<PaginationWrapper<SampleEmployee>> ListAsync(RequestWithPagination<SampleEmployee> request)
        {
            return await ListAsQueryable(request)
                .SelectExclusively(x => new { x.Password })
                .ToPaginatedListAsync(request);
        }

        public override async Task<SampleEmployee> GetAsync(SampleEmployee entity, bool noTracking = false, bool ignoreQueryFilters = false)
        {
            return await GetAsQueryable(entity, noTracking, ignoreQueryFilters)
                .SelectExclusively(x => new { x.Password })
                .FirstOrDefaultAsync();
        }

        public override async Task<SampleEmployee> SaveAsync(SampleEmployee entity, IValidator<SampleEmployee> validator, bool ignoreValidation = false)
        {
            if (!ignoreValidation)
                await ValidateAndThrowAsync(entity, validator);

            SampleEmployee oldRecord = null;
            var primaryId = GetPrimaryId(entity);
            bool isNewRecord = primaryId <= 0;

            if (!isNewRecord)
                oldRecord = GetByIdAsQueryable(primaryId)
                    .Select(x => new SampleEmployee { Password = x.Password })
                    .Single();

            if (!isNewRecord && string.IsNullOrEmpty(entity.Password))
                entity.Password = oldRecord.Password;
            else
                entity.Password = CoreHelper.GetHashedString(entity.Password);

            entity = await base.SaveAsync(entity, null, true);

            entity.Password = string.Empty;

            return entity;
        }
    }
}