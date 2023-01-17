using System;
using CoreType.DBModels;
using CoreData.Common;
using CoreData.DBContexts;
using CoreData.Validators;

namespace CoreData.Repositories
{
    public class CoreCompanyRepository : GenericGenesisRepository<CoreCompany, int, CoreCompanyValidator>
    {
        public CoreCompanyRepository()
        {
        }

        public CoreCompanyRepository(GenesisContextBase context) : base(context)
        {
        }
    }
}