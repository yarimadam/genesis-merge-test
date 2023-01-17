using System;
using CoreSvc.Services;
using CoreType.DBModels;
using CoreData.Repositories;
using CoreData.Validators;

namespace CoreSvc.Services
{
    public class CoreDepartmentService : GenericService<CoreDepartment, int, CoreDepartmentValidator, CoreDepartmentRepository>
    {
    }
}