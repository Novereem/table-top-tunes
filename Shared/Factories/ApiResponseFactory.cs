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
                Message = "Operation completed successfully."
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

        public static ApiResponse<T> CreateFallbackResponse<T>(string errorMessage = "Internal server error, please try again later.")
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = errorMessage
            };
        }
    }
}
