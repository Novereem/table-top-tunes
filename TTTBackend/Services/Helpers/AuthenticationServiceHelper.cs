using Microsoft.IdentityModel.Tokens;
using Shared.Constants;
using Shared.Enums;
using Shared.Interfaces.Data;
using Shared.Interfaces.Services.CommonServices;
using Shared.Models.Common;
using Shared.Models.DTOs;
using Shared.Factories;
using Shared.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Shared.Interfaces.Services.Helpers;

namespace TTTBackend.Services.Helpers
{
    public class AuthenticationServiceHelper : IAuthenticationServiceHelper
    {
        private readonly IAuthenticationData _authData;
        private readonly IPasswordHashingService _passwordHashingService;
        private readonly ILogger<AuthenticationServiceHelper> _logger;

        public AuthenticationServiceHelper(
            IAuthenticationData authData,
            IPasswordHashingService passwordHashingService,
            ILogger<AuthenticationServiceHelper> logger
        )
        {
            _authData = authData;
            _passwordHashingService = passwordHashingService;
            _logger = logger;
        }

        public async Task<ServiceResult<object>> ValidateRegistrationAsync(UserRegistrationDTO registrationDTO)
        {
            if (await _authData.GetUserByUsernameAsync(registrationDTO.Username) != null)
            {
                return PredefinedFailures.GetFailure<object>(ErrorCode.UsernameTaken);
            }

            if (await _authData.GetUserByEmailAsync(registrationDTO.Email) != null)
            {
                return PredefinedFailures.GetFailure<object>(ErrorCode.EmailTaken);
            }

            try
            {
                var addr = new System.Net.Mail.MailAddress(registrationDTO.Email);
                if (addr.Address != registrationDTO.Email)
                    return PredefinedFailures.GetFailure<object>(ErrorCode.InvalidEmailFormat);
            }
            catch
            {
                return PredefinedFailures.GetFailure<object>(ErrorCode.InvalidEmailFormat);
            }

            if (registrationDTO.Password.Length < 5)
            {
                return PredefinedFailures.GetFailure<object>(ErrorCode.PasswordTooShort);
            }
            return ServiceResult<object>.SuccessResult();
        }

        public async Task<ServiceResult<User>> ValidateLoginAsync(UserLoginDTO loginDTO)
        {
            var user = await _authData.GetUserByUsernameAsync(loginDTO.Username);
            if (user == null || !_passwordHashingService.VerifyPassword(loginDTO.Password, user.PasswordHash))
            {
                return PredefinedFailures.GetFailure<User>(ErrorCode.InvalidCredentials);
            }

            return ServiceResult<User>.SuccessResult(user);
        }

        public ServiceResult<string> GenerateJwtToken(Guid userGuid, string username)
        {
            var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
            if (string.IsNullOrEmpty(secretKey))
            {
                return PredefinedFailures.GetFailure<string>(ErrorCode.JWTNullOrEmpty);
            }

            var claims = new[]
            {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.NameIdentifier, userGuid.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: Environment.GetEnvironmentVariable("JWT_ISSUER"),
                audience: Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
                claims: claims,
                expires: DateTime.Now.AddMinutes(1440),
                signingCredentials: creds
            );

            return ServiceResult<string>.SuccessResult(new JwtSecurityTokenHandler().WriteToken(token));
        }
    }
}
