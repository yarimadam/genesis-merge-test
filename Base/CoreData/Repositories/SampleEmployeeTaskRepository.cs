using System;
using CoreType.DBModels;
using CoreData.Common;
using CoreData.DBContexts;
using CoreData.Validators;

namespace CoreData.Repositories
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