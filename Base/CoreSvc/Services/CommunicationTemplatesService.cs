using CoreData.Repositories;
using CoreData.Validators;
using CoreType.DBModels;

namespace CoreSvc.Services
{
    public class CommunicationTemplatesService : GenericService<CommunicationTemplates, int, CommunicationTemplatesValidator, CommunicationTemplatesRepository>
    {
    }
}