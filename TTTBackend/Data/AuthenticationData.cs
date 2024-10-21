using Shared.Interfaces.Data;
using Shared.Models;
using Shared.Models.DTOs;

namespace TTTBackend.Data
{
	public class AuthenticationData// : IAuthenticationData
	{
		private readonly ApplicationDbContext _dbContext;

		public AuthenticationData(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task RegisterUserAsync(User user)
		{
			_dbContext.Users.Add(user);
			await _dbContext.SaveChangesAsync();
		}
	}
}
