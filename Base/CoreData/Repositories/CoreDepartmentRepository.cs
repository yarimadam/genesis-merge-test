using System;
using CoreType.DBModels;
using CoreData.Common;
using CoreData.DBContexts;
using CoreData.Validators;

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