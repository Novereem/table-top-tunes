using Microsoft.EntityFrameworkCore;
using Shared.Interfaces.Data;
using Shared.Models;
using Shared.Models.DTOs;

namespace TTTBackend.Data
{
	public class AuthenticationData : IAuthenticationData
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

		public async Task<User?> GetUserByUsernameAsync(string username)
		{
			return await _dbContext.Users.SingleOrDefaultAsync(u => u.Username == username);
		}

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            return await _dbContext.Users.FindAsync(userId);
        }
    }
}
