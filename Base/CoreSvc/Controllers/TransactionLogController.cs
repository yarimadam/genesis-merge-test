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
    [Resources("TransactionLog_Res")]
    public class TransactionLogController : BaseController
    {
        private readonly TransactionLogService _mainService = new TransactionLogService();

        [HttpPost]
        [ClaimRequirement(ActionType.List)]
        public async Task<ResponseWrapper<PaginationWrapper<TransactionLog>>> List([FromBody] RequestWithPagination<TransactionLog> request)
        {
            return await _mainService.ListAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.GetRecord)]
        public async Task<ResponseWrapper<TransactionLog>> Get([FromBody] TransactionLog request)
        {
            return await _mainService.GetAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Insert)]
        public async Task<ResponseWrapper<TransactionLog>> Insert([FromBody] TransactionLog request)
        {
            return await _mainService.SaveAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Update)]
        public async Task<ResponseWrapper<TransactionLog>> Update([FromBody] TransactionLog request)
        {
            return await _mainService.SaveAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Import)]
        public Task<ResponseWrapper> BulkSave([FromBody] RequestWithExcelData<TransactionLog> request)
        {
            return _mainService.BulkSaveAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Delete)]
        public async Task<ResponseWrapper<bool>> Delete([FromBody] TransactionLog request)
        {
            return await _mainService.DeleteAsync(request);
        }
    }
}