using System.Threading.Tasks;
using Admin.Svc.Services;
using CoreSvc.Common;
using CoreSvc.Filters;
using CoreType.DBModels;
using CoreType.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Admin.Svc.Controllers
{
    [Authorize]
    [DefaultRoute]
    [Resources("TestResource")]
    public class SampleEmployeeTaskController : BaseController
    {
        private readonly SampleEmployeeTaskService _mainService = new SampleEmployeeTaskService();

        [HttpPost]
        [ClaimRequirement(ActionType.List)]
        public async Task<ResponseWrapper<PaginationWrapper<SampleEmployeeTask>>> List([FromBody] RequestWithPagination<SampleEmployeeTask> request)
        {
            return await _mainService.ListAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.GetRecord)]
        public async Task<ResponseWrapper<SampleEmployeeTask>> Get([FromBody] SampleEmployeeTask request)
        {
            return await _mainService.GetAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Insert)]
        public async Task<ResponseWrapper<SampleEmployeeTask>> Insert([FromBody] SampleEmployeeTask request)
        {
            return await _mainService.SaveAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Update)]
        public async Task<ResponseWrapper<SampleEmployeeTask>> Update([FromBody] SampleEmployeeTask request)
        {
            return await _mainService.SaveAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Import)]
        public Task<ResponseWrapper> BulkSave([FromBody] RequestWithExcelData<SampleEmployeeTask> request)
        {
            return _mainService.BulkSaveAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Delete)]
        public async Task<ResponseWrapper<bool>> Delete([FromBody] SampleEmployeeTask request)
        {
            return await _mainService.DeleteAsync(request);
        }
    }
}