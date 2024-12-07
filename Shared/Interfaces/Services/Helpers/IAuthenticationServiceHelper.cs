using Shared.Models.Common;
using Shared.Models.DTOs;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Interfaces.Services.Helpers
{
    public interface IAuthenticationServiceHelper
    {
        Task<ServiceResult<object>> ValidateRegistrationAsync(UserRegistrationDTO registrationDTO);
        Task<ServiceResult<User>> ValidateLoginAsync(UserLoginDTO loginDTO);
        ServiceResult<string> GenerateJwtToken(Guid userGuid, string username);
    }
}
