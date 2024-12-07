using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models.Common.Extensions
{
    public static class ServiceResultExtensions
    {
        public static ApiResponse<T> ToApiResponse<T>(this ServiceResult<T> serviceResult)
        {
            return new ApiResponse<T>
            {
                Success = serviceResult.Success,
                Message = serviceResult.UserMessage ?? string.Empty,
                Data = serviceResult.Success ? serviceResult.Data : default
            };
        }
    }
}
