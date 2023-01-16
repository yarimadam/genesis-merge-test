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
    [Resources("TestResource")] // It should be compatible with pages.js and YourPageNamePageConfig.tsx
    public class SampleEmployeeController : BaseController
    {
        private readonly SampleEmployeeService _mainService = new SampleEmployeeService();

        [HttpPost]
        [ClaimRequirement(ActionType.List)]
        public async Task<ResponseWrapper<PaginationWrapper<SampleEmployee>>> List([FromBody] RequestWithPagination<SampleEmployee> request)
        {
            return await _mainService.ListAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.GetRecord)]
        public async Task<ResponseWrapper<SampleEmployee>> Get([FromBody] SampleEmployee request)
        {
            return await _mainService.GetAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Insert)]
        public async Task<ResponseWrapper<SampleEmployee>> Insert([FromBody] SampleEmployee request)
        {
            return await _mainService.SaveAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Update)]
        public async Task<ResponseWrapper<SampleEmployee>> Update([FromBody] SampleEmployee request)
        {
            return await _mainService.SaveAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Import)]
        public Task<ResponseWrapper> BulkSave([FromBody] RequestWithExcelData<SampleEmployee> request)
        {
            return _mainService.BulkSaveAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Delete)]
        public async Task<ResponseWrapper<bool>> Delete([FromBody] SampleEmployee request)
        {
            return await _mainService.DeleteAsync(request);
        }
    }
}