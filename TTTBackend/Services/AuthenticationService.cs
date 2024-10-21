using Shared.Models.DTOs;
using Shared.Models;
using TTTBackend.Data;
using TTTBackend.Services.CommonServices;
using Shared.Interfaces.Services;

namespace TTTBackend.Services
{
    public class AuthenticationService : IAuthenticationService
	{
		private readonly AuthenticationData _authData;
		private readonly PasswordHashingService _passwordHashingService;

		public AuthenticationService(AuthenticationData authData, PasswordHashingService passwordHashingService)
		{
			_authData = authData;
			_passwordHashingService = passwordHashingService;
		}

		public async Task<(bool Success, string ErrorMessage)> RegisterUserAsync(UserRegistrationDTO registrationDTO)
		{
			try
			{
				// Hash the password
				var passwordHash = _passwordHashingService.HashPassword(registrationDTO.Password);
				var user = new User(registrationDTO.Username, passwordHash);

				// Save the user to the database
				await _authData.RegisterUserAsync(user);
				return (true, "Succesful Registration");
			}
			catch (Exception ex)
			{
				// Handle specific exceptions and log them if necessary
				return (false, ex.Message);
			}
		}

	}
}
