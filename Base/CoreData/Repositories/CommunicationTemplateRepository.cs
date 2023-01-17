using System;
using CoreType.DBModels;
using CoreData.Common;
using CoreData.DBContexts;
using CoreData.Validators;

namespace CoreData.Repositories
{
    public class CommunicationTemplateRepository : GenericGenesisRepository<CommunicationTemplate, int, CommunicationTemplateValidator>
    {
        public CommunicationTemplateRepository()
        {
        }

        public CommunicationTemplateRepository(GenesisContextBase context) : base(context)
        {
        }
    }
}