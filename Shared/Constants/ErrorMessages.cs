using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Constants
{
    public static class ErrorMessages
    {
        private static readonly Dictionary<ErrorCode, string> ErrorMessagesMap = new()
        {
            { ErrorCode.InvalidCredentials, "Invalid credentials provided." },
            { ErrorCode.UsernameTaken, "Username is already taken." },
            { ErrorCode.EmailAlreadyRegistered, "Email is already registered." },
            { ErrorCode.UnauthorizedAccess, "You are not authorized to access this resource." },
            { ErrorCode.InvalidInput, "The provided input is invalid." },
            { ErrorCode.ResourceNotFound, "The requested resource was not found." },
            { ErrorCode.UnknownError, "An unknown error occurred. Please try again later." }
        };

        public static string GetErrorMessage(ErrorCode code)
        {
            return ErrorMessagesMap.TryGetValue(code, out var message) ? message : "An unknown error occurred.";
        }
    }
}
