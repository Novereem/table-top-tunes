using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models.Common
{
    public class ServiceResult<T>
    {
        public T? Data { get; private set; }
        public bool Success { get; private set; }
        public string InternalMessage { get; private set; }
        public string? UserMessage { get; private set; }
        public HttpStatusCode? HttpStatusCode { get; private set; }
        public ErrorCode? ErrorCode { get; private set; }

        private ServiceResult(T? data, bool success, string internalMessage, string? userMessage, HttpStatusCode? httpStatusCode, ErrorCode? errorCode = null)
        {
            Data = data;
            Success = success;
            InternalMessage = internalMessage;
            UserMessage = userMessage;
            HttpStatusCode = httpStatusCode;
            ErrorCode = errorCode;
        }

        public static ServiceResult<T> SuccessResult(T? data = default, string internalMessage = "", string? userMessage = null, HttpStatusCode? httpStatusCode = null) =>
            new ServiceResult<T>(data, true, internalMessage, userMessage, httpStatusCode);

        public static ServiceResult<T> Failure(ErrorCode errorCode, string internalMessage, string? userMessage = null, HttpStatusCode? httpStatusCode = null) =>
            new ServiceResult<T>(default, false, internalMessage, userMessage, httpStatusCode, errorCode);

        public ServiceResult<U> ToFailureResult<U>()
        {
            if (Success)
            {
                throw new InvalidOperationException("Cannot convert a successful result to a failure result.");
            }

            return new ServiceResult<U>(
                default,
                false,
                InternalMessage,
                UserMessage,
                HttpStatusCode,
                ErrorCode
            );
        }
    }
}
