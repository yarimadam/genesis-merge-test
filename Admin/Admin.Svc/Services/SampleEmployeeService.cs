using Admin.Data.Repositories;
using Admin.Data.Validators;
using CoreSvc.Services;
using CoreType.DBModels;

namespace Admin.Svc.Services
{
    public class SampleEmployeeService : GenericService<SampleEmployee, int, SampleEmployeeValidator, SampleEmployeeRepository>
    {
    }
}