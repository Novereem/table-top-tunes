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

namespace TTTBackend.Tests.Services.Unit
{
    public class AudioServiceUnitTests
    {
        private readonly Mock<IAudioData> _mockAudioData;
        private readonly Mock<IUserData> _mockUserData;
        private readonly Mock<ISceneData> _mockSceneData;
        private readonly Mock<IUserClaimsService> _mockUserClaimsService;
        private readonly Mock<ILogger<AudioService>> _mockLogger;
        private readonly AudioService _audioService;

        public AudioServiceUnitTests()
        {
            _mockAudioData = new Mock<IAudioData>();
            _mockUserData = new Mock<IUserData>();
            _mockSceneData = new Mock<ISceneData>();
            _mockUserClaimsService = new Mock<IUserClaimsService>();
            _mockLogger = new Mock<ILogger<AudioService>>();

            _audioService = new AudioService(
                _mockAudioData.Object,
                _mockUserData.Object,
                _mockSceneData.Object,
                _mockUserClaimsService.Object,
                _mockLogger.Object
            );

        }

        [Fact]
        public async Task CreateAudioFileAsync_ShouldReturnSuccess_WhenValidInput()
        {
            var userId = Guid.NewGuid();
            var user = new User("testuser", "test@example.com", "hashedpassword");
            var audioFileDTO = new AudioFileCreateDTO { Name = "Test Audio" };
            var audioFile = new AudioFile { Id = Guid.NewGuid(), Name = audioFileDTO.Name, User = user };

            _mockUserClaimsService.Setup(u => u.GetUserIdFromClaims(It.IsAny<ClaimsPrincipal>()))
                .Returns(ServiceResult<Guid>.SuccessResult(userId));

            _mockUserData.Setup(u => u.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            _mockAudioData.Setup(a => a.SaveAudioFileAsync(It.IsAny<AudioFile>()))
                .Returns(Task.CompletedTask);

            var result = await _audioService.CreateAudioFileAsync(audioFileDTO, new ClaimsPrincipal());

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(audioFileDTO.Name, result.Data.Name);
        }

        [Fact]
        public async Task AssignAudioFileToSceneAsync_ShouldReturnFailure_WhenAudioFileNotFound()
        {
            var assignDTO = new AudioFileAssignDTO { AudioFileId = Guid.NewGuid(), SceneId = Guid.NewGuid() };

            _mockAudioData.Setup(a => a.GetAudioFileByIdAsync(assignDTO.AudioFileId))
                .ReturnsAsync((AudioFile?)null);

            var result = await _audioService.AssignAudioFileToSceneAsync(assignDTO);

            Assert.False(result.Success);
            Assert.Equal(ErrorMessages.GetErrorMessage(ErrorCode.AudioFileNotFound), result.ErrorMessage);
        }

        [Fact]
        public async Task AssignAudioFileToSceneAsync_ShouldReturnSuccess_WhenValidInput()
        {
            var audioFile = new AudioFile { Id = Guid.NewGuid(), Name = "Test Audio" };
            var scene = new Scene { Id = Guid.NewGuid(), Name = "Test Scene" };
            var assignDTO = new AudioFileAssignDTO
            {
                AudioFileId = audioFile.Id,
                SceneId = scene.Id
            };

            _mockAudioData.Setup(a => a.GetAudioFileByIdAsync(assignDTO.AudioFileId))
                .ReturnsAsync(audioFile);

            _mockSceneData.Setup(s => s.GetSceneByIdAsync(assignDTO.SceneId))
                .ReturnsAsync(scene);

            _mockAudioData.Setup(a => a.UpdateAudioFileAsync(It.IsAny<AudioFile>()))
                .Returns(Task.CompletedTask);

            var result = await _audioService.AssignAudioFileToSceneAsync(assignDTO);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(assignDTO.SceneId, result.Data.SceneId);
        }
    }
}
