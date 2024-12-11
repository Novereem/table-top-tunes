using Shared.Constants;
using Shared.Enums;
using Shared.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Factories
{
    public static class ApiResponseFactory
    {
        public static ApiResponse<T> CreateSuccessResponse<T>(T data)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Message = SuccessMessagesUser.GetSuccessMessage(SuccessCodeUser.Success)
            };
        }

        public static ApiResponse<T> CreateErrorResponse<T>(string errorMessage, T? data = default)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = errorMessage,
                Data = data
            };
        }

        public static ApiResponse<T> CreateFallbackResponse<T>()
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = ErrorMessagesUser.GetErrorMessage(ErrorCodeUser.InternalServerError)
            };
        }
        
        public static ApiResponse<T> CreateInvalidSessionResponse<T>()
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = ErrorMessagesUser.GetErrorMessage(ErrorCodeUser.InvalidSession)
            };
        }
    }
}
