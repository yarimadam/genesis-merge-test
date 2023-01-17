using System;
using CoreType.DBModels;
using CoreData.Common;
using CoreData.DBContexts;
using CoreData.Validators;

namespace CoreData.Repositories
{
    public class CommunicationDefinitionRepository : GenericGenesisRepository<CommunicationDefinition, int, CommunicationDefinitionValidator>
    {
        public CommunicationDefinitionRepository()
        {
        }

        public CommunicationDefinitionRepository(GenesisContextBase context) : base(context)
        {
        }
    }
}