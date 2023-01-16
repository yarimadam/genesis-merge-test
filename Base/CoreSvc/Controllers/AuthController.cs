using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CoreData;
using CoreData.CacheManager;
using CoreData.Common;
using CoreData.Infrastructure;
using CoreSvc.Common;
using CoreSvc.Filters;
using CoreSvc.Services;
using CoreType;
using CoreType.DBModels;
using CoreType.Models;
using CoreType.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Serilog;
using static IdentityModel.OidcConstants;

namespace CoreSvc.Controllers
{
    /// <summary>
    /// Admin Login
    /// </summary>
    [Authorize]
    [DefaultRoute]
    public class AuthController : BaseController
    {
        private readonly AuthService _mainService = new AuthService();

        /// <summary>
        /// Gets token from identity server according to requested user
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /getToken
        ///     {
        ///        "Email": "{user_email}",
        ///        "Password": "{user_password}",
        ///     }
        ///
        ///     POST /getToken
        ///     {
        ///        "CustomClientId": "short.client",
        ///        "Email": "{user_email}",
        ///        "Password": "{user_password}",
        ///     }
        /// 
        ///     POST /getToken
        ///     {
        ///        "CustomClientId": "short.client",
        ///        "CustomSecretKey": "{secret}",
        ///        "Email": "{user_email}",
        ///        "Password": "{user_password}",
        ///     }
        /// </remarks>
        /// <param name="request"></param>
        /// <returns>Bearer Token</returns>
        [AllowAnonymous]
        [HttpPost]
        public string GetToken([FromBody] IdentityUserInput request)
        {
            if (string.IsNullOrEmpty(request.CustomClientId))
                request.CustomClientId = Constants.DEFAULT_IDENTITY_CLIENT_ID;
            if (string.IsNullOrEmpty(request.CustomSecretKey))
                request.CustomSecretKey = ConfigurationManager.GetIdentityServerSharedSecret(request.CustomClientId);

            var url = $"{ConfigurationManager.IdentityUrl}/connect/token";

            var dict = new Dictionary<string, string>
            {
                { TokenRequest.GrantType, GrantTypes.Password },
                { TokenRequest.ClientId, request.CustomClientId },
                { TokenRequest.UserName, request.Email },
                { TokenRequest.Password, request.Password },
                { TokenRequest.ClientSecret, request.CustomSecretKey }
            };

            try
            {
                using (HttpClient http = new HttpClient())
                {
                    HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
                    {
                        Content = new FormUrlEncodedContent(dict),
                    };

                    requestMessage.Headers.Clear();

                    HttpContext.Request.Headers
                        .Where(x => x.Key.Equals("Accept-Language") || x.Key.Equals("X-Accept-Language"))
                        .ToList()
                        .ForEach(x => requestMessage.Headers.Add(x.Key, (string) x.Value));

                    HttpContext.Request.Headers
                        .Where(x => x.Key.Equals("X-TenantId"))
                        .ToList()
                        .ForEach(x => requestMessage.Headers.Add(x.Key, (string) x.Value));

                    HttpResponseMessage result = http.SendAsync(requestMessage).Result;

                    var resultStr = result.Content.ReadAsStringAsync().Result;

                    if (result.IsSuccessStatusCode)
                    {
                        JObject responseJson = JObject.Parse(resultStr);
                        var token = responseJson.Value<string>(Constants.ACCESS_TOKEN_NAME);
                        return token;
                    }
                    else
                    {
                        throw new UnauthorizedAccessException(resultStr);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "GetToken");

                throw;
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ResponseWrapper<LoggedInUser>> ForgotPassword([FromBody] LoggedInUser request)
        {
            var genericResponse = new ResponseWrapper<LoggedInUser>();

            var user = _mainService.Repository.ForgotPassword(request);

            if (user != null)
            {
                genericResponse.Data = user;
                genericResponse.Message = LocalizedMessages.PROCESS_SUCCESSFUL;
                genericResponse.Success = true;

                CommunicationManager.Mail.SendAsync("Reset Password", request, genericResponse);

                // Clear sensitive data before returning.
                genericResponse.Data = null;
            }
            else
                genericResponse.Message = LocalizedMessages.PROCESS_FAILED;

            return genericResponse;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ResponseWrapper<LoggedInUser>> ResetPassword([FromBody] LoggedInUser request)
        {
            var genericResponse = new ResponseWrapper<LoggedInUser>();

            var user = _mainService.Repository.ResetPassword(request);

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

        [AllowAnonymous]
        [HttpPost]
        public async Task<ResponseWrapper<string>> GetForgotPasswordEmail([FromBody] LoggedInUser request)
        {
            if (request?.ForgotPasswordKey == null)
                throw new ArgumentNullException(nameof(LoggedInUser.ForgotPasswordKey));

            var genericResponse = new ResponseWrapper<string>();

            var email = _mainService.Repository.GetForgotPasswordEmail(request);

            if (!string.IsNullOrEmpty(email))
            {
                genericResponse.Data = email;
                genericResponse.Message = LocalizedMessages.PROCESS_SUCCESSFUL;
                genericResponse.Success = true;
            }
            else
                genericResponse.Message = LocalizedMessages.PROCESS_FAILED;

            return genericResponse;
        }

        [HttpPost]
        [ClaimRequirement(ActionType.Update, Constants.ResourceCodes.ChangePassword_Res)]
        public async Task<ResponseWrapper<LoggedInUser>> ChangePassword([FromBody] ChangePassword request)
        {
            var genericResponse = new ResponseWrapper<LoggedInUser>();

            var user = _mainService.Repository.ChangePassword(request);

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

        [HttpPost]
        public async Task<ResponseWrapper<LoggedInUser>> ForceUserToChangePassword([FromBody] LoggedInUser request)
        {
            var genericResponse = new ResponseWrapper<LoggedInUser>();

            var user = _mainService.Repository.ForceUserToChangePassword(request);

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

        [AllowAnonymous]
        [HttpPost]
        public async Task<ResponseWrapper<CoreUsers>> Register([FromBody] CoreUsers request)
        {
            var genericResponse = new ResponseWrapper<CoreUsers>();

            CoreUsers user = _mainService.Repository.Register(request);

            if ((int) _mainService.Repository.GetPrimaryId(user) > 0)
            {
                genericResponse.Data = user;
                genericResponse.Message = LocalizedMessages.PROCESS_SUCCESSFUL;
                genericResponse.Success = true;
            }
            else
                genericResponse.Message = LocalizedMessages.PROCESS_FAILED;

            return genericResponse;
        }

        [HttpPost]
        public async Task<ResponseWrapper<AuthorizationClaims>> GetAuthorizationClaimsSchema()
        {
            var genericResponse = new ResponseWrapper<AuthorizationClaims>();

            genericResponse.Data = _mainService.Repository.GetAuthorizationClaimsSchema();
            genericResponse.Message = LocalizedMessages.PROCESS_SUCCESSFUL;
            genericResponse.Success = true;

            return genericResponse;
        }

        [HttpPost]
        [ClaimRequirement(Constants.ResourceCodes.UserPerm_tab, ActionType.GetRecord)]
        public async Task<ResponseWrapper<AuthorizationClaims>> GetAuthorizationClaimsOfUser([FromBody] int userId)
        {
            var genericResponse = new ResponseWrapper<AuthorizationClaims>();

            genericResponse.Data = _mainService.Repository.GetAuthorizationClaims(userId);
            genericResponse.Message = LocalizedMessages.PROCESS_SUCCESSFUL;
            genericResponse.Success = true;

            return genericResponse;
        }

        [HttpPost]
        [ClaimRequirement(Constants.ResourceCodes.UserPerm_tab, ActionType.Update)]
        public async Task<ResponseWrapper> SaveAuthorizationClaimsOfUser([FromBody] UserAuthorizationClaims request)
        {
            var genericResponse = new ResponseWrapper();

            _mainService.Repository.SaveAuthorizationClaims(request);

            DistributedCache.ClearClaims(request.UserId);

            genericResponse.Message = LocalizedMessages.PROCESS_SUCCESSFUL;
            genericResponse.Success = true;

            return genericResponse;
        }

        [HttpPost]
        public async Task<ResponseWrapper<AuthorizationClaims>> GetTemplateById()
        {
            var genericResponse = new ResponseWrapper<AuthorizationClaims>();

            genericResponse.Data = _mainService.Repository.GetAuthorizationClaimsSchema();
            genericResponse.Message = LocalizedMessages.PROCESS_SUCCESSFUL;
            genericResponse.Success = true;

            return genericResponse;
        }
    }
}