using CoreData.Common;
using CoreData.Infrastructure;
using CoreData.Validators;
using CoreType.DBModels;

namespace CoreData.Repositories
{
    public class CoreDepartmentRepository : GenericGenesisRepository<CoreDepartment, int, CoreDepartmentValidator>
    {
        public CoreDepartmentRepository()
        {
        }

        public CoreDepartmentRepository(GenesisContextBase context) : base(context)
        {
        }
    }
}