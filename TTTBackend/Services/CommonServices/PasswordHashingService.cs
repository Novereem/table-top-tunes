using BCrypt.Net;
using Shared.Interfaces.Services.CommonServices;

namespace TTTBackend.Services.CommonServices
{
    public class PasswordHashingService : IPasswordHashingService
	{
		// Hash a password with a salt
		public string HashPassword(string password)
		{
			return BCrypt.Net.BCrypt.HashPassword(password);
		}

		// Verify if the provided password matches the hashed password
		public bool VerifyPassword(string password, string hashedPassword)
		{
			return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
		}
	}
}
