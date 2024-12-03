using Shared.Models.DTOs;
using Shared.Models;
using TTTBackend.Data;
using TTTBackend.Services.CommonServices;
using Shared.Interfaces.Services;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Shared.Interfaces.Data;
using Shared.Models.Extensions;
using Shared.Constants;
using Shared.Enums;
using Shared.Models.Common;
using Shared.Interfaces.Services.CommonServices;

namespace TTTBackend.Services
{
    public class AuthenticationService : IAuthenticationService
	{
		private readonly IAuthenticationData _authData;
		private readonly IPasswordHashingService _passwordHashingService;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(IAuthenticationData authData, IPasswordHashingService passwordHashingService, ILogger<AuthenticationService> logger)

        {
			_authData = authData;
			_passwordHashingService = passwordHashingService;
			_logger = logger;
		}

        public async Task<ServiceResult<string>> RegisterUserAsync(UserRegistrationDTO registrationDTO)
        {
            try
            {
                var newUser = registrationDTO.ToUserFromRegistrationDTO(_passwordHashingService);

                if (await _authData.GetUserByUsernameAsync(newUser.Username) != null)
                {
                    return ServiceResult<string>.Failure(
                        ErrorMessages.GetErrorMessage(ErrorCode.UsernameTaken),
                        HttpStatusCode.Conflict
                    );
                }

                if (await _authData.GetUserByEmailAsync(newUser.Email) != null)
                {
                    return ServiceResult<string>.Failure(
                        ErrorMessages.GetErrorMessage(ErrorCode.EmailAlreadyRegistered),
                        HttpStatusCode.Conflict
                    );
                }

                await _authData.RegisterUserAsync(newUser);
                return ServiceResult<string>.SuccessResult(
                    SuccessMessages.GetSuccessMessage(SuccessCode.Register),
                    HttpStatusCode.Created
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while registering user. Username: {Username}, Email: {Email}", registrationDTO.Username, registrationDTO.Email);
                return ServiceResult<string>.Failure(
                    ErrorMessages.GetErrorMessage(ErrorCode.UnknownError),
                    HttpStatusCode.InternalServerError
                );
            }
        }

        public async Task<(ServiceResult<string>, string? token)> ValidateUserAsync(UserLoginDTO loginDTO)
        {
            try
            {
                var loginUser = loginDTO.ToUserFromLoginDTO(_passwordHashingService);

                var user = await _authData.GetUserByUsernameAsync(loginUser.Username);

                if (user == null || !_passwordHashingService.VerifyPassword(loginDTO.Password, user.PasswordHash))
                {
                    return (
                        ServiceResult<string>.Failure(
                            ErrorMessages.GetErrorMessage(ErrorCode.InvalidCredentials),
                            HttpStatusCode.Unauthorized
                        ),
                        null
                    );
                }

                var token = GenerateJwtToken(user.Id, user.Username);
                return (
                    ServiceResult<string>.SuccessResult(
                        SuccessMessages.GetSuccessMessage(SuccessCode.Login),
                        HttpStatusCode.OK
                    ),
                    token
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while validating the user. Username: {Username}", loginDTO.Username);
                return (
                    ServiceResult<string>.Failure(
                        ErrorMessages.GetErrorMessage(ErrorCode.UnknownError),
                        HttpStatusCode.InternalServerError
                    ),
                    null
                );
            }
        }

        public string GenerateJwtToken(Guid userGuid, string username)
		{
			try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generating JWT token for user: {Username}", username);
                throw new InvalidOperationException(ErrorMessages.GetErrorMessage(ErrorCode.UnknownError), ex);
            }
        }
	}
}
