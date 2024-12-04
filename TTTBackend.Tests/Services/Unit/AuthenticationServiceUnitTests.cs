using Microsoft.Extensions.Logging;
using Moq;
using Shared.Constants;
using Shared.Enums;
using Shared.Interfaces.Data;
using Shared.Interfaces.Services.CommonServices;
using Shared.Models.DTOs;
using Shared.Models;
using TTTBackend.Services;

namespace TTTBackend.Tests.Services.Unit
{
    public class AuthenticationServiceUnitTests
    {
        private readonly Mock<IAuthenticationData> _mockAuthData;
        private readonly Mock<IPasswordHashingService> _mockPasswordHashingService;
        private readonly Mock<ILogger<AuthenticationService>> _mockLogger;
        private readonly AuthenticationService _authService;

        public AuthenticationServiceUnitTests()
        {
            _mockAuthData = new Mock<IAuthenticationData>();
            _mockPasswordHashingService = new Mock<IPasswordHashingService>();
            _mockLogger = new Mock<ILogger<AuthenticationService>>();
            _authService = new AuthenticationService(
                _mockAuthData.Object,
                _mockPasswordHashingService.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldReturnSuccess_WhenValidUserIsRegistered()
        {
            var registrationDTO = new UserRegistrationDTO { Username = "testuser", Email = "test@example.com", Password = "password" };

            _mockAuthData.Setup(a => a.GetUserByUsernameAsync(registrationDTO.Username)).ReturnsAsync((User?)null);
            _mockAuthData.Setup(a => a.GetUserByEmailAsync(registrationDTO.Email)).ReturnsAsync((User?)null);

            var result = await _authService.RegisterUserAsync(registrationDTO);

            Assert.True(result.Success);
            Assert.Equal(SuccessMessages.GetSuccessMessage(SuccessCode.Register), result.Data);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldReturnFailure_WhenUsernameExists()
        {
            var registrationDTO = new UserRegistrationDTO { Username = "existinguser", Email = "test@example.com", Password = "password" };
            var existingUser = new User("existinguser", "existing@example.com", "hashedPassword");

            _mockAuthData.Setup(a => a.GetUserByUsernameAsync(registrationDTO.Username)).ReturnsAsync(existingUser);

            var result = await _authService.RegisterUserAsync(registrationDTO);

            Assert.False(result.Success);
            Assert.Equal(ErrorMessages.GetErrorMessage(ErrorCode.UsernameTaken), result.ErrorMessage);
        }

        [Fact]
        public async Task ValidateUserAsync_ShouldReturnFailure_WhenInvalidCredentialsAreProvided()
        {
            var loginDTO = new UserLoginDTO { Username = "testuser", Password = "wrongpassword" };
            var user = new User("testuser", "test@example.com", "hashedPassword");

            _mockAuthData.Setup(a => a.GetUserByUsernameAsync(loginDTO.Username)).ReturnsAsync(user);
            _mockPasswordHashingService.Setup(p => p.VerifyPassword(loginDTO.Password, user.PasswordHash)).Returns(false);

            var (result, token) = await _authService.ValidateUserAsync(loginDTO);

            Assert.False(result.Success);
            Assert.Null(token);
            Assert.Equal(ErrorMessages.GetErrorMessage(ErrorCode.InvalidCredentials), result.ErrorMessage);
        }
    }
}
