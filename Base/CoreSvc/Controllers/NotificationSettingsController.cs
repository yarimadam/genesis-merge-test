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
    [Resources(Constants.ResourceCodes.NotificationSettings_res)]
    public class NotificationSettingsController : BaseController
    {
        private readonly NotificationSettingsService _mainService = new NotificationSettingsService();

        [HttpPost]
        [ClaimRequirement(ActionType.List)]
        public async Task<ResponseWrapper<PaginationWrapper<NotificationSettings>>> List([FromBody] RequestWithPagination<NotificationSettings> request)
        {
            return await _mainService.ListAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.GetRecord)]
        public async Task<ResponseWrapper<NotificationSettings>> Get([FromBody] NotificationSettings request)
        {
            return await _mainService.GetAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Insert)]
        public async Task<ResponseWrapper<NotificationSettings>> Insert([FromBody] NotificationSettings request)
        {
            return await _mainService.SaveAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Update)]
        public async Task<ResponseWrapper<NotificationSettings>> Update([FromBody] NotificationSettings request)
        {
            return await _mainService.SaveAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Delete)]
        public async Task<ResponseWrapper<bool>> Delete([FromBody] NotificationSettings request)
        {
            return await _mainService.DeleteAsync(request);
        }
    }
}