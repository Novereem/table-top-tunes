using Shared.Models;
using Shared.Models.Common;
using Shared.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Interfaces.Services
{
    public interface IAuthenticationService
    {
		public Task<ServiceResult<RegisterResponseDTO>> RegisterUserAsync(UserRegistrationDTO registrationDTO);
        public Task<ServiceResult<LoginResponseDTO>> ValidateUserAsync(UserLoginDTO loginDTO);
        Task<ServiceResult<User>> GetUserByIdAsync(Guid userId);
    }
}
