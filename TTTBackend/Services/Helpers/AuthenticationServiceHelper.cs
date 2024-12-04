using Microsoft.IdentityModel.Tokens;
using Shared.Constants;
using Shared.Enums;
using Shared.Interfaces.Data;
using Shared.Interfaces.Services.CommonServices;
using Shared.Models.Common;
using Shared.Models.DTOs;
using Shared.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace TTTBackend.Services.Helpers
{
    public class AuthenticationServiceHelper
    {
        private readonly IAuthenticationData _authData;
        private readonly IPasswordHashingService _passwordHashingService;
        private readonly ILogger _logger;

        public AuthenticationServiceHelper(
            IAuthenticationData authData,
            IPasswordHashingService passwordHashingService,
            ILogger logger
        )
        {
            _authData = authData;
            _passwordHashingService = passwordHashingService;
            _logger = logger;
        }

        public async Task<ServiceResult<User>> ValidateRegistrationAsync(UserRegistrationDTO registrationDTO)
        {
            if (await _authData.GetUserByUsernameAsync(registrationDTO.Username) != null)
            {
                _logger.LogWarning("Registration failed: Username already taken. Username: {Username}", registrationDTO.Username);
                return ServiceResult<User>.Failure(ErrorMessages.GetErrorMessage(ErrorCode.UsernameTaken), HttpStatusCode.Conflict);
            }

            if (await _authData.GetUserByEmailAsync(registrationDTO.Email) != null)
            {
                _logger.LogWarning("Registration failed: Email already registered. Email: {Email}", registrationDTO.Email);
                return ServiceResult<User>.Failure(ErrorMessages.GetErrorMessage(ErrorCode.EmailAlreadyRegistered), HttpStatusCode.Conflict);
            }

            return ServiceResult<User>.SuccessResult();
        }

        public async Task<ServiceResult<User>> ValidateLoginAsync(UserLoginDTO loginDTO)
        {
            var user = await _authData.GetUserByUsernameAsync(loginDTO.Username);
            if (user == null || !_passwordHashingService.VerifyPassword(loginDTO.Password, user.PasswordHash))
            {
                _logger.LogWarning("Login failed: Invalid credentials. Username: {Username}", loginDTO.Username);
                return ServiceResult<User>.Failure(ErrorMessages.GetErrorMessage(ErrorCode.InvalidCredentials), HttpStatusCode.Unauthorized);
            }

            return ServiceResult<User>.SuccessResult(user);
        }

        public string GenerateJwtToken(Guid userGuid, string username)
        {
            var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException(ErrorMessages.GetErrorMessage(ErrorCode.JWTNullOrEmpty));
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

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
