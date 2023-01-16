using CoreData.Repositories;
using CoreData.Validators;
using CoreType.DBModels;

namespace CoreSvc.Services
{
    public class CoreDepartmentService : GenericService<CoreDepartment, int, CoreDepartmentValidator, CoreDepartmentRepository>
    {
    }
}