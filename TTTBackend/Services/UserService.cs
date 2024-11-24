using Shared.Interfaces.Data;
using Shared.Interfaces.Services;
using Shared.Models;

namespace TTTBackend.Services
{
    public class UserService : IUserService
    {
        private readonly IUserData _userData;

        public UserService(IUserData userData)
        {
            _userData = userData;
        }

        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            var user = await _userData.GetUserByIdAsync(userId);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            return user;
        }
    }
}
