using Shared.Interfaces.Services.CommonServices;
using Shared.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models.Extensions
{
    public static class AuthenticationExtensions
    {
        public static User ToUserFromLoginDTO(this UserLoginDTO dto, IPasswordHashingService passwordHashingService)
        {
            var hashedPassword = passwordHashingService.HashPassword(dto.Password);

            return new User(
                username: dto.Username,
                email: string.Empty,
                passwordHash: hashedPassword
            );
        }

        public static User ToUserFromRegistrationDTO(this UserRegistrationDTO dto, IPasswordHashingService passwordHashingService)
        {
            var hashedPassword = passwordHashingService.HashPassword(dto.Password);

            return new User(
                username: dto.Username,
                email: dto.Email,
                passwordHash: hashedPassword
            );
        }
    }
}
