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
    [Resources(Constants.ResourceCodes.TransactionLogs_Res)]
    public class TransactionLogsController : BaseController
    {
        private readonly TransactionLogsService _mainService = new TransactionLogsService();

        [HttpPost]
        [ClaimRequirement(ActionType.List)]
        public async Task<ResponseWrapper<PaginationWrapper<TransactionLogs>>> List([FromBody] RequestWithPagination<TransactionLogs> request)
        {
            return await _mainService.ListAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.GetRecord)]
        public async Task<ResponseWrapper<TransactionLogs>> Get([FromBody] TransactionLogs request)
        {
            return await _mainService.GetAsync(request);
        }
    }
}