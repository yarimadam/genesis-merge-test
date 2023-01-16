using System.Collections.Generic;
using System.Linq;
using CoreData.Common;
using CoreData.Infrastructure;
using CoreType.DBModels;

namespace CoreData.Repositories
{
    public class AuthTemplateDetailsRepository : GenericGenesisRepository<AuthTemplateDetail, int>
    {
        public AuthTemplateDetailsRepository()
        {
        }

        public AuthTemplateDetailsRepository(GenesisContextBase context) : base(context)
        {
        }

        public IList<AuthTemplateDetail> List(AuthTemplateDetail entity)
        {
            return DbSet(true)
                .Where(x => x.AuthTemplateId == entity.AuthTemplateId)
                .AddFilters(entity)
                .ToList();
        }

        public bool DeletePassives(AuthTemplate entity)
        {
            var activeRecords = entity.AuthTemplateDetail
                .Where(x => x.Status == 1 && x.AuthTemplateDetailId != default)
                .Select(x => x.AuthTemplateDetailId)
                .ToList();

            DbSet(true)
                .Where(x => x.AuthTemplateId == entity.AuthTemplateId)
                .Where(x => !activeRecords.Contains(x.AuthTemplateDetailId))
                .DeleteFromQuery();

            return true;
        }
    }
}