using System;
using CoreType.DBModels;
using CoreData.Common;
using CoreData.DBContexts;
using CoreData.Validators;

namespace CoreData.Repositories
{
    public class AuthResourceRepository : GenericGenesisRepository<AuthResource, int, AuthResourceValidator>
    {
        public AuthResourceRepository()
        {
        }

        public AuthResourceRepository(GenesisContextBase context) : base(context)
        {
        }
    }
}