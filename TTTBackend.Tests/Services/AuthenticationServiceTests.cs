using Moq;
using System;
using System.Threading.Tasks;
using Xunit;
using TTTBackend.Services;
using Shared.Interfaces.Data;
using TTTBackend.Services.CommonServices;
using Shared.Models.DTOs;
using Shared.Models;
using Shared.Models.Extensions;
using Shared.Constants;
using Shared.Enums;
using Shared.Interfaces.Services.CommonServices;

namespace TTTBackend.Tests.Services
{
    public class AuthenticationServiceTests
    {
        private readonly Mock<IAuthenticationData> _mockAuthData;
        private readonly Mock<IPasswordHashingService> _mockPasswordHashingService;
        private readonly AuthenticationService _authService;

        public AuthenticationServiceTests()
        {
            _mockAuthData = new Mock<IAuthenticationData>();
            _mockPasswordHashingService = new Mock<IPasswordHashingService>();
            _authService = new AuthenticationService(_mockAuthData.Object, _mockPasswordHashingService.Object);
        }

        // Registering Tests
        [Fact]
        public async Task RegisterUserAsync_ShouldReturnSuccess_WhenUserIsRegistered()
        {
            // Arrange
            var registrationDTO = new UserRegistrationDTO { Username = "testuser", Email = "test@example.com", Password = "password" };
            var hashedPassword = "hashedPassword";

            _mockPasswordHashingService.Setup(p => p.HashPassword(registrationDTO.Password)).Returns(hashedPassword);
            _mockAuthData.Setup(a => a.GetUserByUsernameAsync(registrationDTO.Username)).ReturnsAsync((User?)null);
            _mockAuthData.Setup(a => a.GetUserByEmailAsync(registrationDTO.Email)).ReturnsAsync((User?)null);
            _mockAuthData.Setup(a => a.RegisterUserAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            // Act
            var result = await _authService.RegisterUserAsync(registrationDTO);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Successful Registration", result.ErrorMessage);
            _mockAuthData.Verify(a => a.RegisterUserAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldReturnFailure_WhenUsernameAlreadyExists()
        {
            // Arrange
            var registrationDTO = new UserRegistrationDTO { Username = "testuser", Email = "test@example.com", Password = "password" };
            var existingUser = registrationDTO.ToUserFromRegistrationDTO(_mockPasswordHashingService.Object);

            _mockAuthData.Setup(a => a.GetUserByUsernameAsync(registrationDTO.Username)).ReturnsAsync(existingUser);

            // Act
            var result = await _authService.RegisterUserAsync(registrationDTO);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(ErrorMessages.GetErrorMessage(ErrorCode.UsernameTaken), result.ErrorMessage);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldReturnFailure_WhenEmailAlreadyExists()
        {
            // Arrange
            var registrationDTO = new UserRegistrationDTO { Username = "testuser", Email = "test@example.com", Password = "password" };
            var existingUser = registrationDTO.ToUserFromRegistrationDTO(_mockPasswordHashingService.Object);

            _mockAuthData.Setup(a => a.GetUserByEmailAsync(registrationDTO.Email)).ReturnsAsync(existingUser);

            // Act
            var result = await _authService.RegisterUserAsync(registrationDTO);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(ErrorMessages.GetErrorMessage(ErrorCode.EmailAlreadyRegistered), result.ErrorMessage);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldReturnUnknownError_WhenUnexpectedExceptionIsThrown()
        {
            // Arrange
            var registrationDTO = new UserRegistrationDTO
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "password"
            };

            // Mock the method to throw an unexpected exception
            _mockAuthData.Setup(a => a.RegisterUserAsync(It.IsAny<User>()))
                .Throws(new Exception("Unexpected error"));

            // Act
            var result = await _authService.RegisterUserAsync(registrationDTO);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(ErrorMessages.GetErrorMessage(ErrorCode.UnknownError), result.ErrorMessage);
        }

        // Login Tests
        [Fact]
        public async Task ValidateUserAsync_ShouldReturnSuccess_WhenCredentialsAreValid()
        {
            // Arrange
            var loginDTO = new UserLoginDTO { Username = "testuser", Password = "password" };
            var user = new User("testuser", "test@example.com", "hashedPassword");

            _mockAuthData.Setup(a => a.GetUserByUsernameAsync(loginDTO.Username)).ReturnsAsync(user);
            _mockPasswordHashingService.Setup(p => p.VerifyPassword(loginDTO.Password, user.PasswordHash)).Returns(true);

            // Act
            var result = await _authService.ValidateUserAsync(loginDTO);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(user, result.user);
            Assert.Null(result.ErrorMessage);
        }

        [Fact]
        public async Task ValidateUserAsync_ShouldReturnFailure_WhenCredentialsAreInvalid()
        {
            // Arrange
            var loginDTO = new UserLoginDTO { Username = "testuser", Password = "wrongpassword" };
            var user = new User("testuser", "test@example.com", "hashedPassword");

            _mockAuthData.Setup(a => a.GetUserByUsernameAsync(loginDTO.Username)).ReturnsAsync(user);
            _mockPasswordHashingService.Setup(p => p.VerifyPassword(loginDTO.Password, user.PasswordHash)).Returns(false);

            // Act
            var result = await _authService.ValidateUserAsync(loginDTO);

            // Assert
            Assert.False(result.Success);
            Assert.Null(result.user);
            Assert.Equal(ErrorMessages.GetErrorMessage(ErrorCode.InvalidCredentials), result.ErrorMessage);
        }

        [Fact]
        public async Task ValidateUserAsync_ShouldReturnFailure_WhenUserNotFound()
        {
            // Arrange
            var loginDTO = new UserLoginDTO { Username = "nonexistentuser", Password = "password" };

            _mockAuthData.Setup(a => a.GetUserByUsernameAsync(loginDTO.Username)).ReturnsAsync((User?)null);

            // Act
            var result = await _authService.ValidateUserAsync(loginDTO);

            // Assert
            Assert.False(result.Success);
            Assert.Null(result.user); // Ensure no user is returned
            Assert.Equal(ErrorMessages.GetErrorMessage(ErrorCode.InvalidCredentials), result.ErrorMessage);
        }

        // JWT Token Tests
        [Fact]
        public void GenerateJwtToken_ShouldReturnToken_WhenSecretIsSet()
        {
            // Arrange
            Environment.SetEnvironmentVariable("JWT_SECRET_KEY", "cf3894f72d9440798a9542f060988572randomtoken");
            Environment.SetEnvironmentVariable("JWT_ISSUER", "my_issuer");
            Environment.SetEnvironmentVariable("JWT_AUDIENCE", "my_audience");

            var userGuid = Guid.NewGuid();
            var username = "testuser";

            // Act
            var token = _authService.GenerateJwtToken(userGuid, username);

            // Assert
            Assert.False(string.IsNullOrEmpty(token));
        }

        [Fact]
        public void GenerateJwtToken_ShouldThrowException_WhenSecretKeyIsMissing()
        {
            // Arrange
            Environment.SetEnvironmentVariable("JWT_SECRET_KEY", null);

            var userGuid = Guid.NewGuid();
            var username = "testuser";

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => _authService.GenerateJwtToken(userGuid, username));
        }
    }
}