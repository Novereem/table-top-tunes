using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Constants;
using Shared.Enums;
using Shared.Interfaces.Data;
using Shared.Models;
using Shared.Models.DTOs;
using TTTBackend.Data;
using TTTBackend.Services;
using TTTBackend.Services.CommonServices;
using TTTBackend.Services.Helpers;

namespace TTTBackend.Tests.Services.Integration
{
    public class AuthenticationServiceIntegrationTests
    {
        private readonly AuthenticationService _authService;
        private readonly ApplicationDbContext _dbContext;
        private readonly IAuthenticationData _authData;

        public AuthenticationServiceIntegrationTests()
        {
            Environment.SetEnvironmentVariable("JWT_SECRET_KEY", "test-secret-key-that-is-very-long-because-it-needs-to-be");
            Environment.SetEnvironmentVariable("JWT_ISSUER", "test-issuer");
            Environment.SetEnvironmentVariable("JWT_AUDIENCE", "test-audience");

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("TestDatabase_Integration")
                .Options;

            _dbContext = new ApplicationDbContext(options);
            _authData = new AuthenticationData(_dbContext);

            var loggerAuthService = LoggerFactory.Create(builder => { }).CreateLogger<AuthenticationService>();
            var loggerHelper = LoggerFactory.Create(builder => { }).CreateLogger<AuthenticationServiceHelper>();

            var passwordHashingService = new PasswordHashingService();
            var helper = new AuthenticationServiceHelper(_authData, passwordHashingService, loggerHelper);

            _authService = new AuthenticationService(
                _authData,
                passwordHashingService,
                loggerAuthService,
                helper
            );
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldCreateUserSuccessfully()
        {
            var registrationDTO = new UserRegistrationDTO
            {
                Username = "integrationtestuser",
                Email = "integration@example.com",
                Password = "password"
            };

            var result = await _authService.RegisterUserAsync(registrationDTO);

            Assert.True(result.Success);
            Assert.Equal(SuccessMessages.GetSuccessMessage(SuccessCode.Register), result.InternalMessage);
            Assert.NotNull(result.Data);
            Assert.Equal(registrationDTO.Username, result.Data.Username);
        }

        [Fact]
        public async Task ValidateUserAsync_ShouldReturnSuccess_WhenValidCredentialsAreProvided()
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword("password");
            var user = new User("testuser", "test@example.com", hashedPassword);
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var loginDTO = new UserLoginDTO { Username = "testuser", Password = "password" };

            var result = await _authService.ValidateUserAsync(loginDTO);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.NotNull(result.Data.Token);
            Assert.Equal(SuccessMessages.GetSuccessMessage(SuccessCode.Login), result.InternalMessage);
        }
    }
}
