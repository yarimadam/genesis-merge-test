using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;

namespace CoreType.Types
{
    public interface IHttpResponseException
    {
        string Code { get; set; }
        string Message { get; set; }
        string StackTrace { get; set; }
    }

    public class HttpResponseException : IHttpResponseException
    {
        public HttpResponseException() : this(null)
        {
        }

        public HttpResponseException(Exception exception) : this(null, exception)
        {
        }

        public HttpResponseException(string code, Exception exception) : this(code, exception?.Message, exception?.StackTrace)
        {
        }

        public HttpResponseException(string code, string message, string stackTrace = null)
        {
            Code = code;
            Message = message;
            StackTrace = stackTrace;
        }

        public string Code { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }

        public static List<HttpResponseException> BuildExceptionList(Exception exception)
        {
            var exceptionList = new List<HttpResponseException>();

            if (exception is ValidationException validationException)
                return new List<HttpResponseException>(
                    validationException.Errors
                        .Select(validationFailure =>
                            new HttpValidationException(
                                validationFailure.PropertyName,
                                validationFailure.ErrorCode,
                                validationFailure.ErrorMessage,
                                validationException.StackTrace)));

            while (exception != null)
            {
                exceptionList.Add(new HttpResponseException(exception));

                exception = exception.InnerException;
            }

            return exceptionList;
        }
    }
}