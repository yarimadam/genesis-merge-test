using CoreData.Common;
using CoreData.Infrastructure;
using CoreData.Validators;
using CoreType.DBModels;

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