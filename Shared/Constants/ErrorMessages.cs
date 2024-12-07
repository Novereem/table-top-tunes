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
            //Authentication
            { ErrorCode.InvalidCredentials, "Invalid credentials provided." },
            { ErrorCode.UsernameTaken, "Username is already taken." },
            { ErrorCode.EmailTaken, "Email is already registered." },
            { ErrorCode.JWTNullOrEmpty, "JWT secret key is not set in the .env file." },
            { ErrorCode.MissingToken, "Authentication Token Missing." },
            { ErrorCode.UnauthorizedToken, "Invalid UserId claim." },
            { ErrorCode.UserNotFound, "User not found." },

            //Common
            { ErrorCode.UnauthorizedAccess, "You are not authorized to access this resource." },
            { ErrorCode.ResourceNotFound, "The requested resource was not found." },
            { ErrorCode.UnknownError, "An unknown error occurred." },
            { ErrorCode.InternalServerError, "An internal server error occurred." },
            { ErrorCode.Database, "An error occurred in the database." },

            { ErrorCode.CreationFailed, "Creation Failed." },

            //Audio & Scenes
            { ErrorCode.InvalidInput, "The provided input is invalid." },

            //Scenes
            { ErrorCode.SceneNotFound, "Scene not found." },
            
            //Audio
            { ErrorCode.AudioFileNotFound, "Audio file not found." },
        };

        public static string GetErrorMessage(ErrorCode code)
        {
            return ErrorMessagesMap.TryGetValue(code, out var message) ? message : "An unknown error occurred.";
        }
    }
}
