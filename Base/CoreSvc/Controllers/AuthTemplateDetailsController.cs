using System.Collections.Generic;
using System.Threading.Tasks;
using CoreData;
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
    [Resources(Constants.ResourceCodes.AuthTemplateDetails_tab)]
    public class AuthTemplateDetailsController : BaseController
    {
        private readonly AuthTemplateDetailsService _mainService = new AuthTemplateDetailsService();

        [HttpPost]
        [ClaimRequirement(ActionType.List)]
        public async Task<ResponseWrapper<IList<AuthTemplateDetail>>> List([FromBody] RequestWithPagination<AuthTemplateDetail> request)
        {
            var genericResponse = new ResponseWrapper<IList<AuthTemplateDetail>>();

            genericResponse.Data = _mainService.Repository.List(request.Criteria);
            genericResponse.Message = LocalizedMessages.PROCESS_SUCCESSFUL;
            genericResponse.Success = true;

            return genericResponse;
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