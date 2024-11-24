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

namespace TTTBackend.Services
{
    public class AuthenticationService : IAuthenticationService
	{
		private readonly IAuthenticationData _authData;
		private readonly IPasswordHashingService _passwordHashingService;

		public AuthenticationService(IAuthenticationData authData, IPasswordHashingService passwordHashingService)
		{
			_authData = authData;
			_passwordHashingService = passwordHashingService;
		}

        public async Task<(bool Success, string ErrorMessage)> RegisterUserAsync(UserRegistrationDTO registrationDTO)
        {
			try
			{
				var newUser = registrationDTO.ToUserFromRegistrationDTO(_passwordHashingService);

				if (await _authData.GetUserByUsernameAsync(newUser.Username) != null)
				{
					return (false, ErrorMessages.GetErrorMessage(ErrorCode.UsernameTaken));
				}
				if (await _authData.GetUserByEmailAsync(newUser.Email) != null)
				{
					return (false, ErrorMessages.GetErrorMessage(ErrorCode.EmailAlreadyRegistered));
				}

				await _authData.RegisterUserAsync(newUser);
				return (true, "Successful Registration");
			}
			catch (Exception ex)
			{
                return (false, ErrorMessages.GetErrorMessage(ErrorCode.UnknownError));
            }
            
        }

        public async Task<(bool Success, User? user, string? ErrorMessage)> ValidateUserAsync(UserLoginDTO loginDTO)
        {
            var loginUser = loginDTO.ToUserFromLoginDTO(_passwordHashingService);

            var user = await _authData.GetUserByUsernameAsync(loginUser.Username);

            if (user == null || !_passwordHashingService.VerifyPassword(loginDTO.Password, user.PasswordHash))
            {
                return (false, null, ErrorMessages.GetErrorMessage(ErrorCode.InvalidCredentials));
            }

            return (true, user, null);
        }

        public string GenerateJwtToken(Guid userGuid, string username)
		{
			var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
			if (string.IsNullOrEmpty(secretKey))
			{
				throw new InvalidOperationException("JWT secret key is not set in the .env file.");
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
