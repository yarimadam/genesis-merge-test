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
    [Resources("AuthUserRight_Res")]
    public class AuthUserRightController : BaseController
    {
        private readonly AuthUserRightService _mainService = new AuthUserRightService();

        [HttpPost]
        [ClaimRequirement(ActionType.List)]
        public async Task<ResponseWrapper<PaginationWrapper<AuthUserRight>>> List([FromBody] RequestWithPagination<AuthUserRight> request)
        {
            return await _mainService.ListAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.GetRecord)]
        public async Task<ResponseWrapper<AuthUserRight>> Get([FromBody] AuthUserRight request)
        {
            return await _mainService.GetAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Insert)]
        public async Task<ResponseWrapper<AuthUserRight>> Insert([FromBody] AuthUserRight request)
        {
            return await _mainService.SaveAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Update)]
        public async Task<ResponseWrapper<AuthUserRight>> Update([FromBody] AuthUserRight request)
        {
            return await _mainService.SaveAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Import)]
        public Task<ResponseWrapper> BulkSave([FromBody] RequestWithExcelData<AuthUserRight> request)
        {
            return _mainService.BulkSaveAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Delete)]
        public async Task<ResponseWrapper<bool>> Delete([FromBody] AuthUserRight request)
        {
            return await _mainService.DeleteAsync(request);
        }
    }
}