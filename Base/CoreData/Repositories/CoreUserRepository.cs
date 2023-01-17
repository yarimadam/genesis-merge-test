using System;
using CoreType.DBModels;
using CoreData.Common;
using CoreData.DBContexts;
using CoreData.Validators;

namespace CoreData.Repositories
{
    public class CoreUserRepository : GenericGenesisRepository<CoreUser, int, CoreUserValidator>
    {
        public CoreUserRepository()
        {
        }

        public CoreUserRepository(GenesisContextBase context) : base(context)
        {
        }
    }
}