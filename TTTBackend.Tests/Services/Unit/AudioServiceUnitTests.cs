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
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TTTBackend.Services;
using Shared.Factories;
using TTTBackend.Services.Helpers;
using Shared.Interfaces.Services.Helpers;

namespace TTTBackend.Tests.Services.Unit
{
    public class AudioServiceUnitTests
    {
        private readonly Mock<IAudioData> _mockAudioData;
        private readonly Mock<ISceneServiceHelper> _mockSceneHelper;
        private readonly Mock<IAuthenticationService> _mockAuthService;
        private readonly Mock<IUserClaimsService> _mockUserClaimsService;
        private readonly Mock<ILogger<AudioService>> _mockLogger;
        private readonly Mock<IAudioServiceHelper> _mockAudioHelper;
        private readonly AudioService _audioService;

        public AudioServiceUnitTests()
        {
            _mockAudioData = new Mock<IAudioData>();
            _mockSceneHelper = new Mock<ISceneServiceHelper>();
            _mockAuthService = new Mock<IAuthenticationService>();
            _mockUserClaimsService = new Mock<IUserClaimsService>();
            _mockLogger = new Mock<ILogger<AudioService>>();
            _mockAudioHelper = new Mock<IAudioServiceHelper>();

            _audioService = new AudioService(
                _mockAudioData.Object,
                _mockSceneHelper.Object,
                _mockAuthService.Object,
                _mockUserClaimsService.Object,
                _mockLogger.Object,
                _mockAudioHelper.Object
            );
        }

        [Fact]
        public async Task CreateAudioFileAsync_ShouldReturnSuccess_WhenUserAndInputAreValid()
        {
            var userId = Guid.NewGuid();
            var user = new User("testuser", "test@example.com", "passhash");
            var dto = new AudioFileCreateDTO { Name = "Test Audio" };

            _mockUserClaimsService.Setup(u => u.GetUserIdFromClaims(It.IsAny<ClaimsPrincipal>()))
                .Returns(ServiceResult<Guid>.SuccessResult(userId));

            _mockAuthService.Setup(a => a.GetUserByIdAsync(userId))
                .ReturnsAsync(ServiceResult<User>.SuccessResult(user));

            _mockAudioHelper.Setup(h => h.ValidateAudioFileCreateRequest(dto))
                .Returns(ServiceResult<object>.SuccessResult());

            _mockAudioData.Setup(d => d.SaveAudioFileAsync(It.IsAny<AudioFile>()))
                .Returns(Task.CompletedTask);

            var result = await _audioService.CreateAudioFileAsync(dto, null);

            Assert.True(result.Success);
            Assert.Equal(dto.Name, result.Data.Name);
        }

        [Fact]
        public async Task CreateAudioFileAsync_ShouldReturnFailure_WhenUserNotFound()
        {
            var userId = Guid.NewGuid();
            var dto = new AudioFileCreateDTO { Name = "Test Audio" };

            var mockPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }));

            _mockUserClaimsService.Setup(u => u.GetUserIdFromClaims(It.IsAny<ClaimsPrincipal>()))
                .Returns(ServiceResult<Guid>.SuccessResult(userId));

            _mockAuthService.Setup(a => a.GetUserByIdAsync(userId))
                .ReturnsAsync(PredefinedFailures.GetFailure<User>(ErrorCode.ResourceNotFound));

            _mockAudioHelper.Setup(h => h.ValidateAudioFileCreateRequest(It.IsAny<AudioFileCreateDTO>()))
                .Returns(ServiceResult<object>.SuccessResult());

            var result = await _audioService.CreateAudioFileAsync(dto, mockPrincipal);

            Assert.False(result.Success);
            Assert.Equal(ErrorCode.ResourceNotFound, result.ErrorCode);
        }

        [Fact]
        public async Task AssignAudioFileToSceneAsync_ShouldReturnSuccess_WhenValid()
        {
            var assignDTO = new AudioFileAssignDTO { AudioFileId = Guid.NewGuid(), SceneId = Guid.NewGuid() };
            var audioFile = new AudioFile { Id = assignDTO.AudioFileId, Name = "Old Audio" };
            var scene = new Scene { Id = assignDTO.SceneId, Name = "Test Scene" };

            _mockAudioHelper.Setup(h => h.ValidateAudioFileAssignRequest(assignDTO))
                .Returns(ServiceResult<object>.SuccessResult());

            _mockAudioHelper.Setup(h => h.RetrieveAudioFileByIdAsync(assignDTO.AudioFileId))
                .ReturnsAsync(ServiceResult<AudioFile>.SuccessResult(audioFile));

            _mockSceneHelper.Setup(h => h.RetrieveSceneByIdAsync(assignDTO.SceneId))
                .ReturnsAsync(ServiceResult<Scene>.SuccessResult(scene));

            _mockAudioData.Setup(d => d.UpdateAudioFileAsync(It.IsAny<AudioFile>()))
                .Callback<AudioFile>(af => af.Scene = scene)
                .Returns(Task.CompletedTask);

            var result = await _audioService.AssignAudioFileToSceneAsync(assignDTO);

            Assert.True(result.Success);
            Assert.Equal(scene.Id, result.Data.SceneId);
        }
    }
}
