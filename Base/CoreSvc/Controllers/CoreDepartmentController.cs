using System.Threading.Tasks;
using CoreSvc.Common;
using CoreSvc.Filters;
using CoreSvc.Services;
using CoreType.DBModels;
using CoreType.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreSvc.Controllers
{
    [Authorize]
    [DefaultRoute]
    [Resources(Constants.ResourceCodes.CoreDepartment_Res)]
    public class CoreDepartmentController : BaseController
    {
        private readonly CoreDepartmentService _mainService = new CoreDepartmentService();

        [HttpPost]
        [ClaimRequirement(ActionType.List)]
        public async Task<ResponseWrapper<PaginationWrapper<CoreDepartment>>> List([FromBody] RequestWithPagination<CoreDepartment> request)
        {
            return await _mainService.ListAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.GetRecord)]
        public async Task<ResponseWrapper<CoreDepartment>> Get([FromBody] CoreDepartment request)
        {
            return await _mainService.GetAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Insert)]
        public async Task<ResponseWrapper<CoreDepartment>> Insert([FromBody] CoreDepartment request)
        {
            return await _mainService.SaveAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Update)]
        public async Task<ResponseWrapper<CoreDepartment>> Update([FromBody] CoreDepartment request)
        {
            return await _mainService.SaveAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Import)]
        public Task<ResponseWrapper> BulkSave([FromBody] RequestWithExcelData<CoreDepartment> request)
        {
            return _mainService.BulkSaveAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Delete)]
        public async Task<ResponseWrapper<bool>> Delete([FromBody] CoreDepartment request)
        {
            return await _mainService.DeleteAsync(request);
        }
    }
}