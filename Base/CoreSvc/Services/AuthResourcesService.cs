using CoreData.Repositories;
using CoreData.Validators;
using CoreType.DBModels;

namespace CoreSvc.Services
{
    public class AuthResourcesService : GenericService<AuthResources, int, AuthResourcesValidator, AuthResourcesRepository>
    {
    }
}