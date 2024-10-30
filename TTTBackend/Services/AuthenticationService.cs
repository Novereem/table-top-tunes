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
			//Password strength, length and email format are checked in the frontend
			//Email 2 author authentication needs to be implemented in the final product

            if (await _authData.GetUserByUsernameAsync(registrationDTO.Username) != null)
            {
                return (false, "Username is already taken");
            }
            if (await _authData.GetUserByEmailAsync(registrationDTO.Email) != null)
            {
                return (false, "Email is already registered");
            }

            try
			{
				// Hash the password
				var passwordHash = _passwordHashingService.HashPassword(registrationDTO.Password);
				var user = new User(registrationDTO.Username, registrationDTO.Email, passwordHash);

				// Save the user to the database
				await _authData.RegisterUserAsync(user);
				return (true, "Succesful Registration");
			}
			catch (Exception ex)
			{
				return (false, ex.Message);
			}
		}

		public async Task<(bool Success, string Username, string ErrorMessage)> ValidateUserAsync(UserLoginDTO loginDTO)
		{
			var user = await _authData.GetUserByUsernameAsync(loginDTO.Username);

			if (user == null || !_passwordHashingService.VerifyPassword(loginDTO.Password, user.PasswordHash))
			{
				return (false, null, "Invalid credentials");
			}

			return (true, user.Username, null);
		}

		public string GenerateJwtToken(string username)
		{
			var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
			if (string.IsNullOrEmpty(secretKey))
			{
				throw new InvalidOperationException("JWT secret key is not set in the .env file.");
			}

			var claims = new[]
			{
			new Claim(JwtRegisteredClaimNames.Sub, username),
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
		};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: Environment.GetEnvironmentVariable("JWT_ISSUER"),
				audience: Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
				claims: claims,
				expires: DateTime.Now.AddMinutes(30),
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
