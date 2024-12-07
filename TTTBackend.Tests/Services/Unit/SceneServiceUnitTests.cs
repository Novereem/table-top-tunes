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
using Shared.Factories;
using TTTBackend.Services.Helpers;
using Shared.Interfaces.Services.Helpers;

namespace TTTBackend.Tests.Services.Unit
{
    public class SceneServiceUnitTests
    {
        private readonly Mock<ISceneData> _mockSceneData;
        private readonly Mock<IAuthenticationService> _mockAuthService;
        private readonly Mock<IUserClaimsService> _mockUserClaimsService;
        private readonly Mock<ILogger<SceneService>> _mockLogger;
        private readonly Mock<ISceneServiceHelper> _mockHelper;
        private readonly SceneService _sceneService;

        public SceneServiceUnitTests()
        {
            _mockSceneData = new Mock<ISceneData>();
            _mockAuthService = new Mock<IAuthenticationService>();
            _mockUserClaimsService = new Mock<IUserClaimsService>();
            _mockLogger = new Mock<ILogger<SceneService>>();
            _mockHelper = new Mock<ISceneServiceHelper>();

            _sceneService = new SceneService(
                _mockSceneData.Object,
                _mockAuthService.Object,
                _mockUserClaimsService.Object,
                _mockLogger.Object,
                _mockHelper.Object
            );
        }

        [Fact]
        public async Task CreateSceneAsync_ShouldReturnSuccess_WhenSceneCreatedSuccessfully()
        {
            var dto = new SceneCreateDTO { Name = "Test Scene" };
            var userId = Guid.NewGuid();
            var user = new User("testuser", "test@example.com", "hashedpass");
            var createdSceneDTO = new SceneCreateResponseDTO { Id = Guid.NewGuid(), Name = dto.Name };

            _mockUserClaimsService.Setup(u => u.GetUserIdFromClaims(It.IsAny<ClaimsPrincipal>()))
                .Returns(ServiceResult<Guid>.SuccessResult(userId));

            _mockAuthService.Setup(a => a.GetUserByIdAsync(userId))
                .ReturnsAsync(ServiceResult<User>.SuccessResult(user));

            _mockHelper.Setup(h => h.ValidateSceneCreateRequest(dto))
                .Returns(ServiceResult<object>.SuccessResult());

            _mockHelper.Setup(h => h.CreateSceneAsync(dto, user))
                .ReturnsAsync(ServiceResult<SceneCreateResponseDTO>.SuccessResult(createdSceneDTO));

            var result = await _sceneService.CreateSceneAsync(dto, null);

            Assert.True(result.Success);
            Assert.Equal(createdSceneDTO.Id, result.Data.Id);
            Assert.Equal(createdSceneDTO.Name, result.Data.Name);
        }

        [Fact]
        public async Task CreateSceneAsync_ShouldReturnFailure_WhenUserIdNotInClaims()
        {
            var dto = new SceneCreateDTO { Name = "Test Scene" };

            _mockUserClaimsService.Setup(u => u.GetUserIdFromClaims(It.IsAny<ClaimsPrincipal>()))
                .Returns(ServiceResult<Guid>.Failure(ErrorCode.InvalidCredentials, "No user ID"));

            var result = await _sceneService.CreateSceneAsync(dto, null);

            Assert.False(result.Success);
            Assert.Equal(ErrorCode.InvalidCredentials, result.ErrorCode);
        }

        [Fact]
        public async Task CreateSceneAsync_ShouldReturnFailure_WhenUserNotFound()
        {
            var dto = new SceneCreateDTO { Name = "Test Scene" };
            var userId = Guid.NewGuid();

            _mockUserClaimsService.Setup(u => u.GetUserIdFromClaims(It.IsAny<ClaimsPrincipal>()))
                .Returns(ServiceResult<Guid>.SuccessResult(userId));

            _mockAuthService.Setup(a => a.GetUserByIdAsync(userId))
                .ReturnsAsync(PredefinedFailures.GetFailure<User>(ErrorCode.ResourceNotFound));

            var result = await _sceneService.CreateSceneAsync(dto, null);

            Assert.False(result.Success);
            Assert.Equal(ErrorCode.InternalServerError, result.ErrorCode);
        }

        [Fact]
        public async Task GetSceneByIdAsync_ShouldReturnScene_WhenSceneExists()
        {
            var sceneId = Guid.NewGuid();
            var scene = new Scene { Id = sceneId, Name = "Test Scene" };

            _mockHelper.Setup(h => h.RetrieveSceneByIdAsync(sceneId))
                .ReturnsAsync(ServiceResult<Scene>.SuccessResult(scene));

            var result = await _sceneService.GetSceneByIdAsync(sceneId);

            Assert.True(result.Success);
            Assert.Equal(sceneId, result.Data.Id);
        }

        [Fact]
        public async Task GetScenesListByUserIdAsync_ShouldReturnScenes_WhenUserFoundAndScenesExist()
        {
            var userId = Guid.NewGuid();
            var scenes = new List<Scene>
        {
            new Scene { Id = Guid.NewGuid(), Name = "Scene1" },
            new Scene { Id = Guid.NewGuid(), Name = "Scene2" }
        };

            _mockUserClaimsService.Setup(u => u.GetUserIdFromClaims(It.IsAny<ClaimsPrincipal>()))
                .Returns(ServiceResult<Guid>.SuccessResult(userId));

            _mockHelper.Setup(h => h.RetrieveScenesByUserIdAsync(userId))
                .ReturnsAsync(ServiceResult<List<Scene>>.SuccessResult(scenes));

            var result = await _sceneService.GetScenesListByUserIdAsync(null);

            Assert.True(result.Success);
            Assert.Equal(2, result.Data.Count);
        }
    }
}
