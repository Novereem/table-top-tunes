using Microsoft.Extensions.Logging;
using Moq;
using Shared.Constants;
using Shared.Enums;
using Shared.Interfaces.Data;
using Shared.Interfaces.Services.CommonServices;
using Shared.Models.DTOs;
using Shared.Models;
using TTTBackend.Services;
using Shared.Factories;
using Shared.Models.Common;
using TTTBackend.Services.Helpers;
using Shared.Interfaces.Services.Helpers;

namespace TTTBackend.Tests.Services.Unit
{
    public class AuthenticationServiceTests
    {
        private readonly Mock<IAuthenticationData> _mockAuthData;
        private readonly Mock<IPasswordHashingService> _mockPasswordHashingService;
        private readonly Mock<ILogger<AuthenticationService>> _mockLogger;
        private readonly Mock<IAuthenticationServiceHelper> _mockHelper;
        private readonly AuthenticationService _authService;

        public AuthenticationServiceTests()
        {
            _mockAuthData = new Mock<IAuthenticationData>();
            _mockPasswordHashingService = new Mock<IPasswordHashingService>();
            _mockLogger = new Mock<ILogger<AuthenticationService>>();
            _mockHelper = new Mock<IAuthenticationServiceHelper>();

            _authService = new AuthenticationService(
                _mockAuthData.Object,
                _mockPasswordHashingService.Object,
                _mockLogger.Object,
                _mockHelper.Object
            );
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldReturnSuccess_WhenUserIsNew()
        {
            var dto = new UserRegistrationDTO { Username = "newuser", Email = "new@example.com", Password = "Pass123" };

            _mockHelper.Setup(h => h.ValidateRegistrationAsync(dto))
                .ReturnsAsync(ServiceResult<object>.SuccessResult());

            _mockPasswordHashingService.Setup(p => p.HashPassword(dto.Password))
                .Returns("hashedpass");

            _mockAuthData.Setup(a => a.RegisterUserAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            var result = await _authService.RegisterUserAsync(dto);

            Assert.True(result.Success);
            Assert.Equal(HttpStatusCode.Created, result.HttpStatusCode);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldReturnFailure_WhenUsernameTaken()
        {
            var dto = new UserRegistrationDTO { Username = "taken", Email = "test@example.com", Password = "Pass123" };

            _mockHelper.Setup(h => h.ValidateRegistrationAsync(dto))
                .ReturnsAsync(PredefinedFailures.GetFailure<object>(ErrorCode.UsernameTaken));

            var result = await _authService.RegisterUserAsync(dto);

            Assert.False(result.Success);
            Assert.Equal(ErrorCode.UsernameTaken, result.ErrorCode);
        }

        [Fact]
        public async Task ValidateUserAsync_ShouldReturnSuccess_WhenCredentialsAreValid()
        {
            var dto = new UserLoginDTO { Username = "validuser", Password = "Pass123" };
            var user = new User("validuser", "valid@example.com", "hashedpass");

            _mockHelper.Setup(h => h.ValidateLoginAsync(dto))
                .ReturnsAsync(ServiceResult<User>.SuccessResult(user));

            _mockHelper.Setup(h => h.GenerateJwtToken(user.Id, user.Username))
                .Returns(ServiceResult<string>.SuccessResult("valid.jwt.token"));

            var result = await _authService.ValidateUserAsync(dto);

            Assert.True(result.Success);
            Assert.NotNull(result.Data.Token);
        }

        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnFailure_WhenUserNotFound()
        {
            var userId = Guid.NewGuid();

            _mockAuthData.Setup(a => a.GetUserByIdAsync(userId))
                .ReturnsAsync((User?)null);

            var result = await _authService.GetUserByIdAsync(userId);

            Assert.False(result.Success);
            Assert.Equal(ErrorCode.ResourceNotFound, result.ErrorCode);
        }
    }
}
