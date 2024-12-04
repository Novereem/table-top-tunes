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

        public string ErrorMessage { get; private set; }
        public HttpStatusCode? HttpStatusCode { get; private set; }

        private ServiceResult(T? data, bool success, string errorMessage, HttpStatusCode? httpStatusCode)
        {
            Data = data;
            Success = success;
            ErrorMessage = errorMessage;
            HttpStatusCode = httpStatusCode;
        }

        public static ServiceResult<T> SuccessResult(T? data = default, HttpStatusCode? httpStatusCode = null) =>
            new ServiceResult<T>(data, true, string.Empty, httpStatusCode);

        public static ServiceResult<T> Failure(string errorMessage, HttpStatusCode? httpStatusCode = null) =>
            new ServiceResult<T>(default, false, errorMessage, httpStatusCode);
    }
}
