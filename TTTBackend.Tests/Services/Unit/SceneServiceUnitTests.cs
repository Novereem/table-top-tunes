using Microsoft.Extensions.Logging;
using Moq;
using Shared.Constants;
using Shared.Enums;
using Shared.Interfaces.Data;
using Shared.Interfaces.Services.CommonServices;
using Shared.Interfaces.Services;
using Shared.Models.Common;
using Shared.Models.DTOs;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using TTTBackend.Services;

namespace TTTBackend.Tests.Services.Unit
{
    public class SceneServiceUnitTests
    {
        private readonly Mock<ISceneData> _mockSceneData;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IUserClaimsService> _mockUserClaimsService;
        private readonly Mock<ILogger<SceneService>> _mockLogger;
        private readonly SceneService _sceneService;

        public SceneServiceUnitTests()
        {
            _mockSceneData = new Mock<ISceneData>();
            _mockUserService = new Mock<IUserService>();
            _mockUserClaimsService = new Mock<IUserClaimsService>();
            _mockLogger = new Mock<ILogger<SceneService>>();

            _sceneService = new SceneService(
                _mockSceneData.Object,
                _mockUserService.Object,
                _mockUserClaimsService.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task CreateSceneAsync_ShouldReturnSuccess_WhenSceneCreatedSuccessfully()
        {
            var sceneDTO = new SceneCreateDTO { Name = "Test Scene" };
            var userId = Guid.NewGuid();
            var user = new User("testuser", "test@example.com", "hashedpassword");
            var scene = new Scene { Id = Guid.NewGuid(), Name = sceneDTO.Name, User = user };

            _mockUserClaimsService.Setup(u => u.GetUserIdFromClaims(It.IsAny<ClaimsPrincipal>()))
                .Returns(ServiceResult<Guid>.SuccessResult(userId));

            _mockUserService.Setup(u => u.GetUserByIdAsync(userId))
                .ReturnsAsync(ServiceResult<User>.SuccessResult(user));

            _mockSceneData.Setup(s => s.CreateSceneAsync(It.IsAny<Scene>()))
                .ReturnsAsync(scene);

            var result = await _sceneService.CreateSceneAsync(sceneDTO, null);

            Assert.True(result.Success);
            Assert.Equal(scene.Id, result.Data.Id);
            Assert.Equal(scene.Name, result.Data.Name);
        }

        [Fact]
        public async Task CreateSceneAsync_ShouldReturnFailure_WhenUserNotFound()
        {
            var sceneDTO = new SceneCreateDTO { Name = "Test Scene" };
            var userId = Guid.NewGuid();

            _mockUserClaimsService.Setup(u => u.GetUserIdFromClaims(It.IsAny<ClaimsPrincipal>()))
                .Returns(ServiceResult<Guid>.SuccessResult(userId));

            _mockUserService.Setup(u => u.GetUserByIdAsync(userId))
                .ReturnsAsync(ServiceResult<User>.Failure(ErrorMessages.GetErrorMessage(ErrorCode.UserNotFound)));

            var result = await _sceneService.CreateSceneAsync(sceneDTO, null);

            Assert.False(result.Success);
            Assert.Equal(ErrorMessages.GetErrorMessage(ErrorCode.UserNotFound), result.ErrorMessage);
        }

        [Fact]
        public async Task GetSceneByIdAsync_ShouldReturnScene_WhenIdIsValid()
        {
            var sceneId = Guid.NewGuid();
            var scene = new Scene { Id = sceneId, Name = "Test Scene" };

            _mockSceneData.Setup(s => s.GetSceneByIdAsync(sceneId))
                .ReturnsAsync(scene);

            var result = await _sceneService.GetSceneByIdAsync(sceneId);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(sceneId, result.Data.Id);
        }

        [Fact]
        public async Task GetScenesListByUserIdAsync_ShouldReturnScenesList()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var scenes = new List<Scene>
            {
                new Scene { Id = Guid.NewGuid(), Name = "Scene 1" },
                new Scene { Id = Guid.NewGuid(), Name = "Scene 2" }
            };

            var mockClaimsPrincipal = new Mock<ClaimsPrincipal>();
            _mockUserClaimsService.Setup(u => u.GetUserIdFromClaims(mockClaimsPrincipal.Object))
                .Returns(ServiceResult<Guid>.SuccessResult(userId));

            _mockSceneData.Setup(s => s.GetScenesByUserIdAsync(userId))
                .ReturnsAsync(scenes);

            // Act
            var result = await _sceneService.GetScenesListByUserIdAsync(mockClaimsPrincipal.Object);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Count);
        }
    }
}
