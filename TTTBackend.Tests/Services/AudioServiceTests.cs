using Moq;
using Shared.Interfaces.Data;
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
    public class AudioServiceTests
    {
        private readonly Mock<IAudioData> _mockAudioData = new();
        private readonly Mock<IUserData> _mockUserData = new();
        private readonly Mock<ISceneData> _mockSceneData = new();
        private readonly AudioService _audioService;

        public AudioServiceTests()
        {
            _audioService = new AudioService(_mockAudioData.Object, _mockUserData.Object, _mockSceneData.Object);
        }

        //CreateAudioFileAsync
        [Fact]
        public async Task CreateAudioFileAsync_ShouldCreateAudioFileSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User { Id = userId };
            var audioFileCreateDTO = new AudioFileCreateDTO { Name = "Test Audio" };
            var newAudioFile = new AudioFile { Id = Guid.NewGuid(), Name = "Test Audio", User = user };

            _mockUserData.Setup(u => u.GetUserByIdAsync(userId)).ReturnsAsync(user);
            _mockAudioData.Setup(a => a.SaveAudioFileAsync(It.IsAny<AudioFile>()));

            // Act
            var result = await _audioService.CreateAudioFileAsync(audioFileCreateDTO, userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Audio", result.Name);
            _mockUserData.Verify(u => u.GetUserByIdAsync(userId), Times.Once);
            _mockAudioData.Verify(a => a.SaveAudioFileAsync(It.IsAny<AudioFile>()), Times.Once);
        }

        [Fact]
        public async Task CreateAudioFileAsync_ShouldThrowException_IfUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var audioFileCreateDTO = new AudioFileCreateDTO { Name = "Test Audio" };

            _mockUserData.Setup(u => u.GetUserByIdAsync(userId)).ReturnsAsync((User?)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _audioService.CreateAudioFileAsync(audioFileCreateDTO, userId));
            _mockUserData.Verify(u => u.GetUserByIdAsync(userId), Times.Once);
            _mockAudioData.Verify(a => a.SaveAudioFileAsync(It.IsAny<AudioFile>()), Times.Never);
        }

        [Fact]
        public async Task CreateAudioFileAsync_ShouldThrowArgumentException_IfNameIsEmpty()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var audioFileCreateDTO = new AudioFileCreateDTO { Name = "" };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _audioService.CreateAudioFileAsync(audioFileCreateDTO, userId));
            _mockUserData.Verify(u => u.GetUserByIdAsync(It.IsAny<Guid>()), Times.Never);
            _mockAudioData.Verify(a => a.SaveAudioFileAsync(It.IsAny<AudioFile>()), Times.Never);
        }

        //AssignAudioFileToSceneAsync
        [Fact]
        public async Task AssignAudioFileToSceneAsync_ShouldAssignSuccessfully()
        {
            // Arrange
            var audioFileId = Guid.NewGuid();
            var sceneId = Guid.NewGuid();
            var audioFile = new AudioFile { Id = audioFileId, Name = "Test Audio" };
            var scene = new Scene { Id = sceneId, Name = "Test Scene" };
            var assignDTO = new AudioFileAssignDTO { AudioFileId = audioFileId, SceneId = sceneId };

            _mockAudioData.Setup(a => a.GetAudioFileByIdAsync(audioFileId)).ReturnsAsync(audioFile);
            _mockSceneData.Setup(s => s.GetSceneByIdAsync(sceneId)).ReturnsAsync(scene);
            _mockAudioData.Setup(a => a.UpdateAudioFileAsync(It.IsAny<AudioFile>()));

            // Act
            var result = await _audioService.AssignAudioFileToSceneAsync(assignDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(sceneId, result.SceneId);
            _mockAudioData.Verify(a => a.GetAudioFileByIdAsync(audioFileId), Times.Once);
            _mockSceneData.Verify(s => s.GetSceneByIdAsync(sceneId), Times.Once);
            _mockAudioData.Verify(a => a.UpdateAudioFileAsync(It.IsAny<AudioFile>()), Times.Once);
        }

        [Fact]
        public async Task AssignAudioFileToSceneAsync_ShouldThrowException_IfAudioFileNotFound()
        {
            // Arrange
            var audioFileId = Guid.NewGuid();
            var sceneId = Guid.NewGuid();
            var assignDTO = new AudioFileAssignDTO { AudioFileId = audioFileId, SceneId = sceneId };

            _mockAudioData.Setup(a => a.GetAudioFileByIdAsync(audioFileId)).ReturnsAsync((AudioFile?)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _audioService.AssignAudioFileToSceneAsync(assignDTO));
            _mockAudioData.Verify(a => a.GetAudioFileByIdAsync(audioFileId), Times.Once);
            _mockSceneData.Verify(s => s.GetSceneByIdAsync(It.IsAny<Guid>()), Times.Never);
            _mockAudioData.Verify(a => a.UpdateAudioFileAsync(It.IsAny<AudioFile>()), Times.Never);
        }

        [Fact]
        public async Task AssignAudioFileToSceneAsync_ShouldThrowException_IfSceneNotFound()
        {
            // Arrange
            var audioFileId = Guid.NewGuid();
            var sceneId = Guid.NewGuid();
            var audioFile = new AudioFile { Id = audioFileId, Name = "Test Audio" };
            var assignDTO = new AudioFileAssignDTO { AudioFileId = audioFileId, SceneId = sceneId };

            _mockAudioData.Setup(a => a.GetAudioFileByIdAsync(audioFileId)).ReturnsAsync(audioFile);
            _mockSceneData.Setup(s => s.GetSceneByIdAsync(sceneId)).ReturnsAsync((Scene?)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _audioService.AssignAudioFileToSceneAsync(assignDTO));
            _mockAudioData.Verify(a => a.GetAudioFileByIdAsync(audioFileId), Times.Once);
            _mockSceneData.Verify(s => s.GetSceneByIdAsync(sceneId), Times.Once);
            _mockAudioData.Verify(a => a.UpdateAudioFileAsync(It.IsAny<AudioFile>()), Times.Never);
        }
    }
}
