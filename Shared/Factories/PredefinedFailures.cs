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
    public class PredefinedFailures
    {
        private static readonly Dictionary<ErrorCode, (ErrorCodeUser, HttpStatusCode)> Failures = new()
        {
            //Authentication
            { ErrorCode.EmailTaken, (ErrorCodeUser.EmailTaken, HttpStatusCode.Conflict) },
            { ErrorCode.UsernameTaken, (ErrorCodeUser.UsernameTaken, HttpStatusCode.Conflict) },
            { ErrorCode.InvalidCredentials, (ErrorCodeUser.InvalidCredentials, HttpStatusCode.Unauthorized) },
            { ErrorCode.JWTNullOrEmpty, (ErrorCodeUser.InternalServerError, HttpStatusCode.InternalServerError) },
            { ErrorCode.UnauthorizedToken, (ErrorCodeUser.InternalServerError, HttpStatusCode.InternalServerError) },

            //Scenes & Audio
            { ErrorCode.InvalidInput, (ErrorCodeUser.InternalServerError, HttpStatusCode.InternalServerError) },

            //Audio
            { ErrorCode.AudioFileNotFound, (ErrorCodeUser.InternalServerError, HttpStatusCode.InternalServerError) },


            //Common
            { ErrorCode.UnknownError, (ErrorCodeUser.UnknownError, HttpStatusCode.InternalServerError) },
            { ErrorCode.InternalServerError, (ErrorCodeUser.InternalServerError, HttpStatusCode.InternalServerError) },
            { ErrorCode.ResourceNotFound, (ErrorCodeUser.InternalServerError, HttpStatusCode.InternalServerError) },

            { ErrorCode.CreationFailed, (ErrorCodeUser.CreationFailed, HttpStatusCode.InternalServerError) },
        };

        public static ServiceResult<T> GetFailure<T>(ErrorCode errorCode)
        {
            if (!Failures.TryGetValue(errorCode, out var failure))
            {
                throw new KeyNotFoundException($"No predefined failure found for ErrorCode: {errorCode}");
            }

            var (userErrorCode, httpStatusCode) = failure;

            return ServiceResult<T>.Failure(
                errorCode,
                ErrorMessages.GetErrorMessage(errorCode),
                ErrorMessagesUser.GetErrorMessage(userErrorCode),
                httpStatusCode
            );
        }
    }
}
