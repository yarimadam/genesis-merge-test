using System;
using System.Net;
using System.Threading.Tasks;
using CoreData.CacheManager;
using CoreData.Common;
using CoreType.Types;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Serilog;

namespace CoreSvc.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "HandleExceptionAsync");

                if (!context.Response.HasStarted)
                    await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception error)
        {
            HttpStatusCode code;
            var response = new ResponseWrapper();

            if (error is SecurityTokenExpiredException)
            {
                code = HttpStatusCode.Unauthorized;
                response.Message = await DistributedCache.GetAsync("Unauthorized");
                response.Data = new { authenticated = false, tokenExpired = true };
            }
            else if (error is ValidationException validationException)
            {
                code = HttpStatusCode.UnprocessableEntity;
                response.Message = validationException.Message;
                response.AddError(error);
            }
            else
            {
                code = HttpStatusCode.InternalServerError;
                response.Message = error.Message;
                response.AddError(error);
                // response.Data = new { error.InnerException, error.Message, error.StackTrace };
            }

            var result = JsonConvert.SerializeObject(response, CustomJsonSerializerSettings.MVC);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int) code;

            await context.Response.WriteAsync(result);
        }
    }
}