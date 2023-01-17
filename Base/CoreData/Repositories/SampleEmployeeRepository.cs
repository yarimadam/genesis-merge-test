using System;
using CoreType.DBModels;
using CoreData.Common;
using CoreData.DBContexts;
using CoreData.Validators;

namespace CoreData.Repositories
{
    public class SampleEmployeeRepository : GenericGenesisRepository<SampleEmployee, int, SampleEmployeeValidator>
    {
        public SampleEmployeeRepository()
        {
        }

        public SampleEmployeeRepository(GenesisContextBase context) : base(context)
        {
        }
    }
}