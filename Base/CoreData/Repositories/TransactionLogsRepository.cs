using System;
using System.Linq;
using System.Threading.Tasks;
using CoreData.Common;
using CoreData.Infrastructure;
using CoreType.DBModels;
using CoreType.Types;
using FluentValidation;

namespace CoreData.Repositories
{
    public class TransactionLogsRepository : GenericGenesisRepository<TransactionLogs, int>
    {
        public TransactionLogsRepository()
        {
        }

        public TransactionLogsRepository(GenesisContextBase context) : base(context)
        {
        }

        public override async Task<PaginationWrapper<TransactionLogs>> ListAsync(RequestWithPagination<TransactionLogs> request)
        {
            var query = DbSet(true);

            if (!string.IsNullOrEmpty(request.Criteria.ServiceUrl))
                query = query.Where(x => x.ServiceUrl.ToLower().Contains(request.Criteria.ServiceUrl.ToLower()));

            if (request.Criteria.LogDateBegin > DateTime.MinValue)
                query = query.Where(x => x.LogDateBegin >= request.Criteria.LogDateBegin.Date);

            if (request.Criteria.LogDateEnd > DateTime.MinValue)
            {
                DateTime endDate = request.Criteria.LogDateEnd.Value.Date.AddDays(1).AddSeconds(-1);
                query = query.Where(x => x.LogDateEnd <= endDate);
            }

            if (request.Criteria.UserId > 0)
                query = query.Where(x => x.UserId == request.Criteria.UserId);

            if (request.Criteria.Status > 0)
                query = query.Where(x => x.Status == request.Criteria.Status);

            if (request.Criteria.StatusCode > 0)
                query = query.Where(x => x.StatusCode == request.Criteria.StatusCode);

            return await query
                .AddSortings(request)
                .ThenByDescending(x => x.LogId)
                .ToPaginatedListAsync(request);
        }

        public override async Task<TransactionLogs> SaveAsync(TransactionLogs entity, IValidator<TransactionLogs> validator, bool ignoreValidation = false)
        {
            entity.ServiceUrl = entity.ServiceUrl.Trim('/');
            return await base.SaveAsync(entity, validator, ignoreValidation);
        }
    }
}