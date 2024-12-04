using Shared.Constants;
using Shared.Enums;
using Shared.Interfaces.Data;
using Shared.Interfaces.Services;
using Shared.Models;
using Shared.Models.Common;

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

        public async Task<ServiceResult<User>> GetUserByIdAsync(Guid userId)
        {
            var user = await _userData.GetUserByIdAsync(userId);

            if (user == null)
            {
                return ServiceResult<User>.Failure(
                    ErrorMessages.GetErrorMessage(ErrorCode.UserNotFound),
                    HttpStatusCode.NotFound
                );
            }

            return ServiceResult<User>.SuccessResult(user, HttpStatusCode.OK);
        }
    }
}
