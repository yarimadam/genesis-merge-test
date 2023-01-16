using System.Linq;
using System.Threading.Tasks;
using CoreData.Common;
using CoreData.Infrastructure;
using CoreData.Validators;
using CoreType.DBModels;
using CoreType.Types;
using Microsoft.EntityFrameworkCore;

namespace CoreData.Repositories
{
    public class NotificationRepository : GenericGenesisRepository<Notification, int, NotificationValidator>
    {
        public NotificationRepository()
        {
        }

        public NotificationRepository(GenesisContextBase context) : base(context)
        {
        }

        public override async Task<PaginationWrapper<Notification>> ListAsync(RequestWithPagination<Notification> request)
        {
            request.Criteria.UserId = Session.GetUserId();

            return await ListAsQueryable(request)
                .ThenByDescending(x => x.CreatedDate)
                .ToPaginatedListAsync(request);
        }

        public async Task<PaginationWrapper<Notification>> ListDetails(RequestWithPagination<Notification> entity)
        {
            entity.Criteria.UserId = Session.GetUserId();

            return await DbSet(true)
                .Include(x => x.NotificationSettings)
                .AddFilters(entity)
                .Where(x => x.Status != 3)
                .AddSortings(entity)
                .ThenByDescending(x => x.CreatedDate)
                .SelectExclusively(e => e.NotificationSettings.Data)
                .ToPaginatedListAsync(entity);
        }

        public override async Task<Notification> GetAsync(Notification entity, bool noTracking = false, bool ignoreQueryFilters = false)
        {
            return await GetAsQueryable(entity, noTracking, ignoreQueryFilters)
                .Include(x => x.NotificationSettings)
                .FirstOrDefaultAsync();
        }
    }
}