using Moq;
using Shared.Interfaces.Data;
using Shared.Interfaces.Services;
using Shared.Models.DTOs;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTTBackend.Services;

namespace TTTBackend.Tests.Services
{
    public class SceneServiceTests
    {
        private readonly Mock<ISceneData> _mockSceneData;
        private readonly Mock<IUserService> _mockUserService;
        private readonly SceneService _sceneService;

        public SceneServiceTests()
        {
            _mockSceneData = new Mock<ISceneData>();
            _mockUserService = new Mock<IUserService>();
            _sceneService = new SceneService(_mockSceneData.Object, _mockUserService.Object);
        }

        [Fact]
        public async Task CreateSceneAsync_ShouldCreateSceneSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User("testuser", "test@example.com", "hashedpassword");
            var sceneDTO = new SceneCreateDTO { Name = "Test Scene" };
            var scene = new Scene { Id = Guid.NewGuid(), Name = sceneDTO.Name, User = user, CreatedAt = DateTime.UtcNow };

            _mockUserService.Setup(u => u.GetUserByIdAsync(userId)).ReturnsAsync(user);
            _mockSceneData.Setup(s => s.CreateSceneAsync(It.IsAny<Scene>())).ReturnsAsync(scene);

            // Act
            var result = await _sceneService.CreateSceneAsync(sceneDTO, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(scene.Id, result.Id);
            Assert.Equal(scene.Name, result.Name);
            _mockUserService.Verify(u => u.GetUserByIdAsync(userId), Times.Once);
            _mockSceneData.Verify(s => s.CreateSceneAsync(It.IsAny<Scene>()), Times.Once);
        }

        [Fact]
        public async Task GetSceneByIdAsync_ShouldReturnScene_WhenIdIsValid()
        {
            // Arrange
            var sceneId = Guid.NewGuid();
            var scene = new Scene { Id = sceneId, Name = "Test Scene", CreatedAt = DateTime.UtcNow };
            _mockSceneData.Setup(s => s.GetSceneByIdAsync(sceneId)).ReturnsAsync(scene);

            // Act
            var result = await _sceneService.GetSceneByIdAsync(sceneId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(scene.Id, result.Id);
            Assert.Equal(scene.Name, result.Name);
            _mockSceneData.Verify(s => s.GetSceneByIdAsync(sceneId), Times.Once);
        }

        [Fact]
        public async Task GetSceneByIdAsync_ShouldReturnNull_WhenSceneNotFound()
        {
            // Arrange
            var sceneId = Guid.NewGuid();
            _mockSceneData.Setup(s => s.GetSceneByIdAsync(sceneId)).ReturnsAsync((Scene?)null);

            // Act
            var result = await _sceneService.GetSceneByIdAsync(sceneId);

            // Assert
            Assert.Null(result);
            _mockSceneData.Verify(s => s.GetSceneByIdAsync(sceneId), Times.Once);
        }

        [Fact]
        public async Task GetScenesListByUserIdAsync_ShouldReturnScenesList()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var scenes = new List<Scene>
            {
                new Scene { Id = Guid.NewGuid(), Name = "Scene 1", CreatedAt = DateTime.UtcNow },
                new Scene { Id = Guid.NewGuid(), Name = "Scene 2", CreatedAt = DateTime.UtcNow }
            };
            _mockSceneData.Setup(s => s.GetScenesByUserIdAsync(userId)).ReturnsAsync(scenes);

            // Act
            var result = await _sceneService.GetScenesListByUserIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Scene 1", result[0].Name);
            Assert.Equal("Scene 2", result[1].Name);
            _mockSceneData.Verify(s => s.GetScenesByUserIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetScenesListByUserIdAsync_ShouldReturnEmptyList_WhenNoScenesExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockSceneData.Setup(s => s.GetScenesByUserIdAsync(userId)).ReturnsAsync(new List<Scene>());

            // Act
            var result = await _sceneService.GetScenesListByUserIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockSceneData.Verify(s => s.GetScenesByUserIdAsync(userId), Times.Once);
        }
    }
}
