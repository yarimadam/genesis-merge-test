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
    [Resources("Notification_Res")]
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
        [ClaimRequirement(ActionType.Import)]
        public Task<ResponseWrapper> BulkSave([FromBody] RequestWithExcelData<Notification> request)
        {
            return _mainService.BulkSaveAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Delete)]
        public async Task<ResponseWrapper<bool>> Delete([FromBody] Notification request)
        {
            return await _mainService.DeleteAsync(request);
        }
    }
}