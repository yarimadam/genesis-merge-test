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
    [Resources(Constants.ResourceCodes.Notification_res)]
    public class NotificationController : BaseController
    {
        private readonly NotificationService _mainService = new NotificationService();

        [HttpPost]
        [ClaimRequirement(ActionType.List)]
        public async Task<ResponseWrapper<PaginationWrapper<Notification>>> List([FromBody] RequestWithPagination<Notification> request)
        {
            return await _mainService.ListAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.List)]
        public async Task<ResponseWrapper<PaginationWrapper<Notification>>> ListDetails([FromBody] RequestWithPagination<Notification> request)
        {
            var genericResponse = new ResponseWrapper<PaginationWrapper<Notification>>();

            genericResponse.Data = await _mainService.Repository.ListDetails(request);
            genericResponse.Message = LocalizedMessages.PROCESS_SUCCESSFUL;
            genericResponse.Success = true;

            return genericResponse;
        }

        [HttpPost]
        [ClaimRequirement(ActionType.GetRecord)]
        public async Task<ResponseWrapper<Notification>> Get([FromBody] Notification request)
        {
            return await _mainService.GetAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Insert)]
        public async Task<ResponseWrapper<Notification>> Insert([FromBody] Notification request)
        {
            return await _mainService.SaveAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Update)]
        public async Task<ResponseWrapper<Notification>> Update([FromBody] Notification request)
        {
            return await _mainService.SaveAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Delete)]
        public async Task<ResponseWrapper<bool>> Delete([FromBody] Notification request)
        {
            return await _mainService.DeleteAsync(request);
        }
    }
}