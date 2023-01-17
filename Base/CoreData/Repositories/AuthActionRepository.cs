using System;
using CoreType.DBModels;
using CoreData.Common;
using CoreData.DBContexts;
using CoreData.Validators;

namespace CoreData.Repositories
{
    public class AuthActionRepository : GenericGenesisRepository<AuthAction, int, AuthActionValidator>
    {
        public AuthActionRepository()
        {
        }

        public AuthActionRepository(GenesisContextBase context) : base(context)
        {
        }
    }
}