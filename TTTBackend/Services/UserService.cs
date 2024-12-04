using Shared.Interfaces.Data;
using Shared.Interfaces.Services;
using Shared.Models;

namespace TTTBackend.Services
{
    public class UserService : IUserService
    {
        private readonly IUserData _userData;
        private readonly ILogger<AuthenticationService> _logger;

        public UserService(IUserData userData, ILogger<AuthenticationService> logger)
        {
            _userData = userData;
            _logger = logger;
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
