using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Constants
{
    public static class ErrorMessagesUser
    {
        private static readonly Dictionary<ErrorCodeUser, string> ErrorMessagesUserMap = new()
        {
            //Authentication
            { ErrorCodeUser.UsernameTaken, "Username is already taken." },
            { ErrorCodeUser.EmailTaken, "Email is already registered." },
            { ErrorCodeUser.InvalidCredentials, "Wrong username or password." },
            { ErrorCodeUser.InvalidSession, "Session expired, please log in again to continue using the app." },

            //Audio & Scenes
            { ErrorCodeUser.InvalidInput, "The provided input is invalid, please use the correct syntax." },

            //Common
            { ErrorCodeUser.UnknownError, "An unknown error occurred, please try again later." },
            { ErrorCodeUser.InternalServerError, "An internal server error occurred, please try again later." },

            { ErrorCodeUser.CreationFailed, "Failed to create." }
        };

        public static string GetErrorMessage(ErrorCodeUser code)
        {
            return ErrorMessagesUserMap.TryGetValue(code, out var message) ? message : "An unknown error occurred, please try again later";
        }
    }
}
