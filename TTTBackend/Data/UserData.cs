using Shared.Interfaces.Data;
using Shared.Models;

namespace TTTBackend.Data
{
    public class UserData : IUserData
    {
        private readonly ApplicationDbContext _context;

        public UserData(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            return await _context.Users.FindAsync(userId);
        }
    }
}
