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
    [Resources(Constants.ResourceCodes.CommunicationDefinitions_tab)]
    public class CommunicationDefinitionsController : BaseController
    {
        private readonly CommunicationDefinitionsService _mainService = new CommunicationDefinitionsService();

        [HttpPost]
        [ClaimRequirement(ActionType.List)]
        public async Task<ResponseWrapper<PaginationWrapper<CommunicationDefinitions>>> List([FromBody] RequestWithPagination<CommunicationDefinitions> request)
        {
            return await _mainService.ListAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.GetRecord)]
        public async Task<ResponseWrapper<CommunicationDefinitions>> Get([FromBody] CommunicationDefinitions request)
        {
            return await _mainService.GetAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Insert)]
        public async Task<ResponseWrapper<CommunicationDefinitions>> Insert([FromBody] CommunicationDefinitions request)
        {
            return await _mainService.SaveAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Update)]
        public async Task<ResponseWrapper<CommunicationDefinitions>> Update([FromBody] CommunicationDefinitions request)
        {
            return await _mainService.SaveAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Import)]
        public Task<ResponseWrapper> BulkSave([FromBody] RequestWithExcelData<CommunicationDefinitions> request)
        {
            return _mainService.BulkSaveAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Delete)]
        public async Task<ResponseWrapper<bool>> Delete([FromBody] CommunicationDefinitions request)
        {
            return await _mainService.DeleteAsync(request);
        }
    }
}