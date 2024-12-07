using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Enums
{
    public enum ErrorCodeUser
    {
        //Authentication
        UsernameTaken,
        EmailTaken,
        InvalidCredentials,
        InvalidSession,

        //Audio & Scene
        InvalidInput,

        //Common
        InternalServerError,
        UnknownError,

        CreationFailed,
    }
}
