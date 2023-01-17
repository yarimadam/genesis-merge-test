using System;
using CoreType.DBModels;
using CoreData.Common;
using CoreData.DBContexts;
using CoreData.Validators;

namespace CoreData.Repositories
{
    public class AuthUserRightRepository : GenericGenesisRepository<AuthUserRight, int, AuthUserRightValidator>
    {
        public AuthUserRightRepository()
        {
        }

        public AuthUserRightRepository(GenesisContextBase context) : base(context)
        {
        }
    }
}