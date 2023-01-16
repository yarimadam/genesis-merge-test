using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CoreData;
using CoreData.CacheManager;
using CoreData.Common;
using CoreData.Infrastructure.Producers;
using CoreType.DBModels;
using CoreType.Types;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Serilog;

namespace CoreSvc.Filters
{
    public class GlobalLoggingFilter : IAsyncActionFilter
    {
        private readonly Stopwatch _sw = new Stopwatch();

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var requestBody = string.Empty;
            var originalRequestBody = string.Empty;
            var responseBody = string.Empty;
            var transactionLog = new TransactionLogs();
            Exception exception = null;
            ActionExecutedContext resultContext = null;

            try
            {
                try
                {
                    transactionLog.LogDateBegin = DateTime.UtcNow;

                    var tempRequestBody = context.ActionArguments.Count <= 1
                        ? context.ActionArguments.Values.FirstOrDefault()
                        : context.ActionArguments;

                    if (tempRequestBody != null)
                        requestBody = JsonConvert.SerializeObject(tempRequestBody, CustomJsonSerializerSettings.Logging);
                    else
                        originalRequestBody = await context.HttpContext.Request.ReadBodyAsync();

                    _sw.Restart();

                    // next() calls the action method.
                    resultContext = await next();
                }
                catch (Exception e)
                {
                    exception = e;
                }
                finally
                {
                    _sw.Stop();
                }

                var result = resultContext?.Result;

                responseBody = JsonConvert.SerializeObject(result, CustomJsonSerializerSettings.Logging);
            }
            catch (Exception e)
            {
                if (exception != null)
                    exception = e;
            }
            finally
            {
                if (resultContext?.Exception != null)
                    exception = resultContext.Exception;

                try
                {
                    if (resultContext?.Canceled == true)
                        await resultContext.Result.ExecuteResultAsync(resultContext);

                    await FillTransactionLog(transactionLog, context.HttpContext, requestBody, originalRequestBody, responseBody, exception);

                    LogProducer.Produce(transactionLog);
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex, "GlobalLoggingFilter.OnActionExecutionAsync", transactionLog);
                }

                exception?.ReThrow();
            }
        }

        private async Task FillTransactionLog(TransactionLogs transactionLog, HttpContext context, string requestBody, string originalRequestBody, string responseBody, Exception exception)
        {
            var statusCode = context.Response.StatusCode;
            var userId = context.User.FindFirstValue(JwtClaimTypes.Subject);

            transactionLog.UserId = userId != null ? Convert.ToInt32(userId) : 0;
            transactionLog.ServiceUrl = context.Request.Path;
            transactionLog.Request = FormatRequest(context, requestBody, originalRequestBody, transactionLog.ServiceUrl);
            transactionLog.Response = FormatResponse(responseBody, _sw.Elapsed.TotalMilliseconds, exception);
            transactionLog.StatusCode = statusCode;

            dynamic simplifiedClaims = null;
            if (transactionLog.UserId > 0)
            {
                var allClaims = await DistributedCache.GetClaimsAsync(transactionLog.UserId);
                simplifiedClaims = Helper.SimplifyClaims(allClaims);
            }

            transactionLog.CurrentClaims = simplifiedClaims;
            transactionLog.Status = exception != null ? 3 : (statusCode >= 200 && statusCode < 300) || statusCode == 302 ? 1 : 2;
            transactionLog.LogDateEnd = DateTime.UtcNow;
        }

        private static RequestLog FormatRequest(HttpContext context, string requestBody, string originalRequestBody, string serviceUrl)
        {
            var request = context.Request;

            return new RequestLog
            {
                ServiceUrlFull = $"{request.Scheme}://{request.Host}" + serviceUrl,
                RequestBody = requestBody.Length < Constants.MAX_LOG_CHAR_LENGTH ? JsonConvert.DeserializeObject(requestBody) : LocalizedMessages.REQUEST_LENGTH_EXCEEDED.ToString(requestBody.Length),
                OriginalRequestBody = string.IsNullOrEmpty(requestBody)
                    ? (originalRequestBody.Length < Constants.MAX_LOG_CHAR_LENGTH
                        ? JsonConvert.DeserializeObject(originalRequestBody)
                        : LocalizedMessages.REQUEST_LENGTH_EXCEEDED.ToString(originalRequestBody.Length))
                    : null,
                UserAgent = request.Headers.FirstOrDefault(x => x.Key.Equals(HeaderNames.UserAgent)).Value,
                Referrer = request.Headers.FirstOrDefault(x => x.Key.Equals(HeaderNames.Referer)).Value,
                RemoteIP = context.Connection.RemoteIpAddress.ToString()
            };
        }

        private static ResponseLog FormatResponse(string data, double elapsedMilliseconds, Exception exception)
        {
            object responseBody;

            try
            {
                responseBody = data.Length < Constants.MAX_LOG_CHAR_LENGTH ? JsonConvert.DeserializeObject(data) : LocalizedMessages.RESPONSE_LENGTH_EXCEEDED.ToString(data.Length);
            }
            catch
            {
                responseBody = data;
            }

            return new ResponseLog
            {
                ResponseBody = responseBody,
                ErrorMessage = exception?.Message ?? string.Empty,
                ErrorMessageDetail = exception != null ? (exception.InnerException + "\n\n" + exception.StackTrace) : string.Empty,
                ProcessElapsedTime = elapsedMilliseconds
            };
        }
    }
}