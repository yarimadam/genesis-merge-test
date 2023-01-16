using Admin.Data.Repositories;
using Admin.Data.Validators;
using CoreSvc.Services;
using CoreType.DBModels;

namespace Admin.Svc.Services
{
    public class SampleEmployeeTaskService : GenericService<SampleEmployeeTask, int, SampleEmployeeTaskValidator, SampleEmployeeTaskRepository>
    {
    }
}