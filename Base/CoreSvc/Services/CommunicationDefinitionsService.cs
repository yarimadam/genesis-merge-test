using CoreData.Repositories;
using CoreData.Validators;
using CoreType.DBModels;

namespace CoreSvc.Services
{
    public class CommunicationDefinitionsService : GenericService<CommunicationDefinitions, int, CommunicationDefinitionsValidator, CommunicationDefinitionsRepository>
    {
    }
}