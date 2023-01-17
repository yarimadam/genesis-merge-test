using System.Threading.Tasks;
using CoreSvc.Filters;
using CoreType.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CoreType.DBModels;
using CoreSvc.Services;

namespace CoreSvc.Controllers
{
    [Authorize]
    [DefaultRoute]
    [Resources("AuthTemplateDetail_Res")]
    public class AuthTemplateDetailController : BaseController
    {
        private readonly AuthTemplateDetailService _mainService = new AuthTemplateDetailService();

        [HttpPost]
        [ClaimRequirement(ActionType.List)]
        public async Task<ResponseWrapper<PaginationWrapper<AuthTemplateDetail>>> List([FromBody] RequestWithPagination<AuthTemplateDetail> request)
        {
            return await _mainService.ListAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.GetRecord)]
        public async Task<ResponseWrapper<AuthTemplateDetail>> Get([FromBody] AuthTemplateDetail request)
        {
            return await _mainService.GetAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Insert)]
        public async Task<ResponseWrapper<AuthTemplateDetail>> Insert([FromBody] AuthTemplateDetail request)
        {
            return await _mainService.SaveAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Update)]
        public async Task<ResponseWrapper<AuthTemplateDetail>> Update([FromBody] AuthTemplateDetail request)
        {
            return await _mainService.SaveAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Import)]
        public Task<ResponseWrapper> BulkSave([FromBody] RequestWithExcelData<AuthTemplateDetail> request)
        {
            return _mainService.BulkSaveAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Delete)]
        public async Task<ResponseWrapper<bool>> Delete([FromBody] AuthTemplateDetail request)
        {
            return await _mainService.DeleteAsync(request);
        }
    }
}