using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Enums
{
    public enum ErrorCode
    {
        InvalidCredentials,
        UsernameTaken,
        EmailAlreadyRegistered,
        UnauthorizedAccess,
        InvalidInput,
        ResourceNotFound,
        Database,
        JWTNullOrEmpty,
        MissingToken,
        UnauthorizedToken,
        UserNotFound,
        AudioFileNotFound,
        SceneNotFound,
        UnknownError
    }
}
