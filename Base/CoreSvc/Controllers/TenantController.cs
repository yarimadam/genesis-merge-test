using System.Collections.Generic;
using System.Threading.Tasks;
using CoreData;
using CoreSvc.Common;
using CoreSvc.Filters;
using CoreSvc.Services;
using CoreType;
using CoreType.DBModels;
using CoreType.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreSvc.Controllers
{
    [Authorize]
    [DefaultRoute]
    [Resources(Constants.ResourceCodes.Tenant_Res)]
    public class TenantController : BaseController
    {
        private readonly TenantService _mainService = new TenantService();

        [HttpPost]
        [ClaimRequirement(ActionType.List)]
        public async Task<ResponseWrapper<PaginationWrapper<Tenant>>> List([FromBody] RequestWithPagination<Tenant> request)
        {
            return await _mainService.ListAsync(request);
        }

        [HttpPost]
        public async Task<ResponseWrapper<List<Tenant>>> ListTenantAndSubTenants([FromBody] int? tenantId)
        {
            var genericResponse = new ResponseWrapper<List<Tenant>>();

            tenantId ??= Session.CurrentUser.TenantId;

            genericResponse.Data = _mainService.Repository.ListTenantAndSubTenants(tenantId.Value);
            genericResponse.Message = LocalizedMessages.PROCESS_SUCCESSFUL;
            genericResponse.Success = true;

            return genericResponse;
        }

        [HttpPost]
        public async Task<ResponseWrapper<List<Tenant>>> SearchTenantAndSubTenants([FromBody] SearchTenantInput request)
        {
            var genericResponse = new ResponseWrapper<List<Tenant>>();

            request.TenantId ??= Session.CurrentUser.TenantId;

            genericResponse.Data = _mainService.Repository.SearchTenantAndSubTenants(request.TenantId.Value, request.SearchString);
            genericResponse.Message = LocalizedMessages.PROCESS_SUCCESSFUL;
            genericResponse.Success = true;

            return genericResponse;
        }

        [HttpPost]
        [ClaimRequirement(ActionType.GetRecord)]
        public async Task<ResponseWrapper<Tenant>> Get([FromBody] Tenant request)
        {
            return await _mainService.GetAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Insert)]
        public async Task<ResponseWrapper<Tenant>> Insert([FromBody] Tenant request)
        {
            return await _mainService.SaveAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Update, Constants.ResourceCodes.Profile_Res)]
        public async Task<ResponseWrapper<Tenant>> Update([FromBody] Tenant request)
        {
            return await _mainService.SaveAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Import)]
        public Task<ResponseWrapper> BulkSave([FromBody] RequestWithExcelData<Tenant> request)
        {
            return _mainService.BulkSaveAsync(request);
        }
    }
}