using Admin.Data.Validators;
using CoreData.Common;
using CoreData.Infrastructure;
using CoreType.DBModels;

namespace Admin.Data.Repositories
{
    public class SampleEmployeeTaskRepository : GenericGenesisRepository<SampleEmployeeTask, int, SampleEmployeeTaskValidator>
    {
        public SampleEmployeeTaskRepository()
        {
        }

        public SampleEmployeeTaskRepository(GenesisContextBase context) : base(context)
        {
        }
    }
}