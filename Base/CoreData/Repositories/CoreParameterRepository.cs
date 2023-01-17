using System;
using CoreType.DBModels;
using CoreData.Common;
using CoreData.DBContexts;
using CoreData.Validators;

namespace CoreData.Repositories
{
    public class CoreParameterRepository : GenericGenesisRepository<CoreParameter, int, CoreParameterValidator>
    {
        public CoreParameterRepository()
        {
        }

        public CoreParameterRepository(GenesisContextBase context) : base(context)
        {
        }
    }
}