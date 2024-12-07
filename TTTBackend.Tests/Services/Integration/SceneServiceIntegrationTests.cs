using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Models.DTOs;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTTBackend.Data;
using TTTBackend.Services;
using System.Security.Claims;
using TTTBackend.Services.CommonServices;
using Shared.Interfaces.Data;
using Shared.Interfaces.Services.CommonServices;
using TTTBackend.Services.Helpers;
using Shared.Interfaces.Services;

namespace TTTBackend.Tests.Services.Integration
{
    public class SceneServiceIntegrationTests
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ISceneData _sceneData;
        private readonly IAuthenticationService _authService;
        private readonly IUserClaimsService _userClaimsService;
        private readonly SceneService _sceneService;

        public SceneServiceIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("SceneTestDb")
                .Options;

            _dbContext = new ApplicationDbContext(options);
            _sceneData = new SceneData(_dbContext);
            _authService = new AuthenticationService(
                new AuthenticationData(_dbContext),
                new PasswordHashingService(),
                LoggerFactory.Create(builder => { }).CreateLogger<AuthenticationService>(),
                new AuthenticationServiceHelper(
                    new AuthenticationData(_dbContext),
                    new PasswordHashingService(),
                    LoggerFactory.Create(builder => { }).CreateLogger<AuthenticationServiceHelper>()
                )
            );
            _userClaimsService = new UserClaimsService();

            var helper = new SceneServiceHelper(
                _sceneData,
                LoggerFactory.Create(builder => { }).CreateLogger<SceneServiceHelper>()
            );

            _sceneService = new SceneService(
                _sceneData,
                _authService,
                _userClaimsService,
                LoggerFactory.Create(builder => { }).CreateLogger<SceneService>(),
                helper
            );
        }

        [Fact]
        public async Task CreateSceneAsync_ShouldCreateSceneSuccessfully()
        {
            var user = new User("integrationUser", "int@example.com", BCrypt.Net.BCrypt.HashPassword("password"));
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            }));

            var dto = new SceneCreateDTO { Name = "Integration Scene" };

            var result = await _sceneService.CreateSceneAsync(dto, userPrincipal);

            Assert.True(result.Success);
            Assert.Equal("Integration Scene", result.Data.Name);
        }

        [Fact]
        public async Task GetScenesListByUserIdAsync_ShouldReturnEmptyList_WhenNoScenesExist()
        {
            var user = new User("emptyUser", "empty@example.com", BCrypt.Net.BCrypt.HashPassword("password"));
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            }));

            var result = await _sceneService.GetScenesListByUserIdAsync(userPrincipal);

            Assert.True(result.Success);
            Assert.Empty(result.Data);
        }
    }
}
