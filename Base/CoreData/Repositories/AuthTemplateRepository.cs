using System;
using CoreType.DBModels;
using CoreData.Common;
using CoreData.DBContexts;
using CoreData.Validators;

namespace CoreData.Repositories
{
    public class AuthTemplateRepository : GenericGenesisRepository<AuthTemplate, int, AuthTemplateValidator>
    {
        public AuthTemplateRepository()
        {
        }

        public AuthTemplateRepository(GenesisContextBase context) : base(context)
        {
        }
    }
}