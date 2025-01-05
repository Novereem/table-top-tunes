using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Enums
{
    public enum ErrorCode
    {
        //Authentication
        InvalidCredentials,
        UsernameTaken,
        EmailTaken,
        JWTNullOrEmpty,
        MissingToken,
        UnauthorizedToken,
        UserNotFound,
        InvalidEmailFormat,
        PasswordTooShort,

        //Audio & Scene
        InvalidInput,

        //Audio
        AudioFileNotFound,

        //Scene
        SceneNotFound,

        //Common
        ResourceNotFound,
        UnauthorizedAccess,
        Database,
        UnknownError,
        InternalServerError,

        CreationFailed
    }
}
