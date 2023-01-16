using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace CoreType.Types
{
    public class ResponseWrapper<T> : ActionResult // Generic Action Result
    {
        public bool Success { get; set; }
        public List<HttpResponseException> Errors { get; set; } = new List<HttpResponseException>();
        public string Message { get; set; } = string.Empty;
        public T Data { get; set; }

        public static implicit operator ResponseWrapper<T>(ResponseWrapper responseWrapper) => responseWrapper?.ToGeneric<T>();
        public static implicit operator ResponseWrapper<dynamic>(ResponseWrapper<T> responseWrapper) => responseWrapper?.ToDynamic();

        public override Task ExecuteResultAsync(ActionContext context)
        {
            var result = new ObjectResult(this);
            var executor = context.HttpContext.RequestServices.GetRequiredService<IActionResultExecutor<ObjectResult>>();

            return executor.ExecuteAsync(context, result);
        }

        public ResponseWrapper ToDynamic() => new ResponseWrapper
        {
            Data = Data,
            Errors = Errors,
            Message = Message,
            Success = Success,
        };

        public ResponseWrapper<T> AddError(Exception exception)
        {
            Errors.AddRange(HttpResponseException.BuildExceptionList(exception));

            return this;
        }

        public ResponseWrapper<T> AddError(string message)
        {
            return AddError(string.Empty, message);
        }

        public ResponseWrapper<T> AddError(string code, string message)
        {
            Errors.Add(new HttpResponseException(code, message));

            return this;
        }
    }

    public class ResponseWrapper : ResponseWrapper<dynamic>
    {
        public ResponseWrapper<T> ToGeneric<T>() => new ResponseWrapper<T>
        {
            Data = (T) (object) Data,
            Errors = Errors,
            Message = Message,
            Success = Success,
        };
    }
}