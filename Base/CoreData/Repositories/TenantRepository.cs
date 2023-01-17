using System;
using CoreType.DBModels;
using CoreData.Common;
using CoreData.DBContexts;
using CoreData.Validators;

namespace CoreData.Repositories
{
    public class TenantRepository : GenericGenesisRepository<Tenant, int, TenantValidator>
    {
        public TenantRepository()
        {
        }

        public TenantRepository(GenesisContextBase context) : base(context)
        {
        }
    }
}