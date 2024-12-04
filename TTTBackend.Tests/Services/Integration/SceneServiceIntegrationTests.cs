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

namespace TTTBackend.Tests.Services.Integration
{
    public class SceneServiceIntegrationTests
    {
        private readonly SceneService _sceneService;
        private readonly ApplicationDbContext _dbContext;


        public SceneServiceIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "SceneServiceTestDB")
                .Options;

            _dbContext = new ApplicationDbContext(options);

            var sceneData = new SceneData(_dbContext);
            var userData = new UserData(_dbContext);
            var userService = new UserService(userData, null);

            var userClaimsService = new UserClaimsService();

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });
            var logger = loggerFactory.CreateLogger<SceneService>();

            _sceneService = new SceneService(sceneData, userService, userClaimsService, logger);
        }

        [Fact]
        public async Task CreateSceneAsync_ShouldCreateSceneSuccessfully()
        {
            // Arrange
            var user = new User("integrationtestuser", "integration@example.com", "hashedpassword");
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "test"));

            Assert.NotNull(claimsPrincipal);
            Assert.NotEmpty(claimsPrincipal.Claims);

            var sceneDTO = new SceneCreateDTO { Name = "Integration Test Scene" };
            var userInDb = await _dbContext.Users.FindAsync(user.Id);
            Assert.NotNull(userInDb);

            // Act
            var result = await _sceneService.CreateSceneAsync(sceneDTO, claimsPrincipal);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(sceneDTO.Name, result.Data.Name);

            // Verify the scene exists in the database
            var createdScene = await _dbContext.Scenes.FindAsync(result.Data.Id);
            Assert.NotNull(createdScene);
            Assert.Equal(sceneDTO.Name, createdScene.Name);
            Assert.Equal(user.Id, createdScene.User.Id);
        }

        [Fact]
        public async Task GetSceneByIdAsync_ShouldReturnScene_WhenSceneExists()
        {
            var user = new User("integrationtestuser", "integration@example.com", "hashedpassword");
            var scene = new Scene { Name = "Integration Test Scene", User = user, CreatedAt = DateTime.UtcNow };

            _dbContext.Users.Add(user);
            _dbContext.Scenes.Add(scene);
            await _dbContext.SaveChangesAsync();

            var result = await _sceneService.GetSceneByIdAsync(scene.Id);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(scene.Name, result.Data.Name);
        }

        [Fact]
        public async Task GetScenesListByUserIdAsync_ShouldReturnScenesForUser()
        {
            // Arrange
            var user = new User("integrationtestuser", "integration@example.com", "hashedpassword");
            var scenes = new List<Scene>
            {
                new Scene { Name = "Scene 1", User = user },
                new Scene { Name = "Scene 2", User = user }
            };

            _dbContext.Users.Add(user);
            _dbContext.Scenes.AddRange(scenes);
            await _dbContext.SaveChangesAsync();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "test"));

            // Act
            var result = await _sceneService.GetScenesListByUserIdAsync(claimsPrincipal);

            // Assert
            Assert.True(result.Success, "Expected success but got failure.");
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Count);
            Assert.Contains(result.Data, s => s.Name == "Scene 1");
            Assert.Contains(result.Data, s => s.Name == "Scene 2");
        }
    }
}
