using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Constants;
using Shared.Enums;
using Shared.Interfaces.Data;
using Shared.Models;
using Shared.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTTBackend.Data;
using TTTBackend.Services;
using TTTBackend.Services.CommonServices;

namespace TTTBackend.Tests.Services.Integration
{
    public class AuthenticationServiceIntegrationTests
    {
        private readonly AuthenticationService _authService;
        private readonly ApplicationDbContext _dbContext;
        private readonly IAuthenticationData _authData;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationServiceIntegrationTests()
        {
            // Set environment variables for JWT configuration
            Environment.SetEnvironmentVariable("JWT_SECRET_KEY", "test-secret-key-that-is-very-long-because-it-needs-to-be");
            Environment.SetEnvironmentVariable("JWT_ISSUER", "test-issuer");
            Environment.SetEnvironmentVariable("JWT_AUDIENCE", "test-audience");

            // Set up an in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _dbContext = new ApplicationDbContext(options);
            _authData = new AuthenticationData(_dbContext);

            // Set up a logger
            _logger = new LoggerFactory().CreateLogger<AuthenticationService>();

            // Create the service
            _authService = new AuthenticationService(
                _authData,
                new PasswordHashingService(),
                _logger
            );
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldCreateUserSuccessfully()
        {
            var registrationDTO = new UserRegistrationDTO
            {
                Username = "integrationtest",
                Email = "integration@example.com",
                Password = "password"
            };

            var result = await _authService.RegisterUserAsync(registrationDTO);

            Assert.True(result.Success);
            Assert.Equal(SuccessMessages.GetSuccessMessage(SuccessCode.Register), result.Data);
        }

        [Fact]
        public async Task ValidateUserAsync_ShouldReturnSuccess_WhenValidCredentialsAreProvided()
        {
            // Arrange
            var user = new User("testuser", "test@example.com", BCrypt.Net.BCrypt.HashPassword("password"));
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var loginDTO = new UserLoginDTO { Username = "testuser", Password = "password" };

            // Act
            var (result, token) = await _authService.ValidateUserAsync(loginDTO);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(token);
            Assert.Equal(SuccessMessages.GetSuccessMessage(SuccessCode.Login), result.Data);
        }
    }
}
