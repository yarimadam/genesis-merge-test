using CoreData.Repositories;
using CoreData.Validators;
using CoreType.DBModels;

namespace CoreSvc.Services
{
    public class TenantService : GenericService<Tenant, int, TenantValidator, TenantRepository>
    {
    }
}