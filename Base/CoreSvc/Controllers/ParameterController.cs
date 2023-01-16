using System;
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
    [Resources(Constants.ResourceCodes.SystemParametersInfo_tab)]
    public class ParameterController : BaseController
    {
        private readonly ParameterService _mainService = new ParameterService();

        [HttpPost]
        [ClaimRequirement(ActionType.List)]
        public async Task<ResponseWrapper<PaginationWrapper<CoreParameters>>> List([FromBody] RequestWithPagination<CoreParameters> request)
        {
            return await _mainService.ListAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.GetRecord)]
        public async Task<ResponseWrapper<CoreParameters>> Get([FromBody] CoreParameters request)
        {
            return await _mainService.GetAsync(request);
        }

        [HttpPost]
        public async Task<ResponseWrapper<IList<CoreParameters>>> GetByKey([FromBody] CoreParameters request)
        {
            if (request?.KeyCode == null)
                throw new ArgumentNullException(nameof(request.KeyCode));

            var genericResponse = new ResponseWrapper<IList<CoreParameters>>();

            genericResponse.Data = _mainService.Repository.GetByKey(request);
            genericResponse.Message = LocalizedMessages.PROCESS_SUCCESSFUL;
            genericResponse.Success = true;

            return genericResponse;
        }

        [HttpPost]
        public async Task<ResponseWrapper<string>> GetLocalizedParam([FromBody] CoreParameters request)
        {
            var genericResponse = new ResponseWrapper<string>();

            genericResponse.Data = _mainService.Repository.GetLocalizedParam(request.KeyCode, Session.PreferredLocale);
            genericResponse.Message = LocalizedMessages.PROCESS_SUCCESSFUL;
            genericResponse.Success = true;

            return genericResponse;
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Insert)]
        public async Task<ResponseWrapper<CoreParameters>> Insert([FromBody] CoreParameters request)
        {
            return await _mainService.SaveAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Update)]
        public async Task<ResponseWrapper<CoreParameters>> Update([FromBody] CoreParameters request)
        {
            return await _mainService.SaveAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Import)]
        public Task<ResponseWrapper> BulkSave([FromBody] RequestWithExcelData<CoreParameters> request)
        {
            return _mainService.BulkSaveAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Delete)]
        public async Task<ResponseWrapper<bool>> Delete([FromBody] CoreParameters request)
        {
            return await _mainService.DeleteAsync(request);
        }
    }
}