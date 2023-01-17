using System;
using CoreType.DBModels;
using CoreData.Common;
using CoreData.DBContexts;
using CoreData.Validators;

namespace CoreData.Repositories
{
    public class AuthTemplateDetailRepository : GenericGenesisRepository<AuthTemplateDetail, int, AuthTemplateDetailValidator>
    {
        public AuthTemplateDetailRepository()
        {
        }

        public AuthTemplateDetailRepository(GenesisContextBase context) : base(context)
        {
        }
    }
}