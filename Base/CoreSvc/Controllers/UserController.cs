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
    [Resources(Constants.ResourceCodes.UserInfo_tab)]
    public class UserController : BaseController
    {
        private readonly UserService _mainService = new UserService();

        [HttpPost]
        [ClaimRequirement(ActionType.List)]
        public async Task<ResponseWrapper<PaginationWrapper<CoreUsers>>> List([FromBody] RequestWithPagination<CoreUsers> request)
        {
            return await _mainService.ListAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.GetRecord)]
        public async Task<ResponseWrapper<CoreUsers>> Get([FromBody] CoreUsers request)
        {
            return await _mainService.GetAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Insert)]
        public async Task<ResponseWrapper<CoreUsers>> Insert([FromBody] CoreUsers request)
        {
            return await _mainService.SaveAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Update)]
        public async Task<ResponseWrapper<CoreUsers>> Update([FromBody] CoreUsers request)
        {
            return await _mainService.SaveAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Import)]
        public Task<ResponseWrapper> BulkSave([FromBody] RequestWithExcelData<CoreUsers> request)
        {
            return _mainService.BulkSaveAsync(request);
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Delete)]
        public async Task<ResponseWrapper<bool>> Delete([FromBody] CoreUsers request)
        {
            return await _mainService.DeleteAsync(request);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ResponseWrapper<CoreUsers>> VerifyUser([FromBody] CoreUsers request)
        {
            var genericResponse = new ResponseWrapper<CoreUsers>();

            var user = _mainService.Repository.VerifyUser(request);

            if (user != null)
            {
                genericResponse.Data = user;
                genericResponse.Message = LocalizedMessages.PROCESS_SUCCESSFUL;
                genericResponse.Success = true;
            }
            else
                genericResponse.Message = LocalizedMessages.PROCESS_FAILED;

            return genericResponse;
        }
    }
}