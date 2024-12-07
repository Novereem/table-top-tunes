using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Models.DTOs;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TTTBackend.Data;
using TTTBackend.Services.CommonServices;
using TTTBackend.Services;
using Shared.Interfaces.Data;
using TTTBackend.Services.Helpers;

namespace TTTBackend.Tests.Services.Integration
{
    public class AudioServiceIntegrationTests
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IAudioData _audioData;
        private readonly SceneServiceHelper _sceneHelper;
        private readonly AudioServiceHelper _audioHelper;
        private readonly AudioService _audioService;
        private readonly User _testUser;

        public AudioServiceIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("AudioIntegrationTestDb")
                .Options;
            _dbContext = new ApplicationDbContext(options);
            _audioData = new AudioData(_dbContext);
            var sceneData = new SceneData(_dbContext);

            _sceneHelper = new SceneServiceHelper(
                sceneData,
                LoggerFactory.Create(builder => { }).CreateLogger<SceneServiceHelper>()
            );
            _audioHelper = new AudioServiceHelper(
                _audioData,
                LoggerFactory.Create(builder => { }).CreateLogger<AudioServiceHelper>()
            );

            var authService = new AuthenticationService(
                new AuthenticationData(_dbContext),
                new PasswordHashingService(),
                LoggerFactory.Create(builder => { }).CreateLogger<AuthenticationService>(),
                new AuthenticationServiceHelper(
                    new AuthenticationData(_dbContext),
                    new PasswordHashingService(),
                    LoggerFactory.Create(builder => { }).CreateLogger<AuthenticationServiceHelper>()
                )
            );
            var userClaimsService = new UserClaimsService();

            _audioService = new AudioService(
                _audioData,
                _sceneHelper,
                authService,
                userClaimsService,
                LoggerFactory.Create(builder => { }).CreateLogger<AudioService>(),
                _audioHelper
            );

            _testUser = new User("intUser", "int@example.com", BCrypt.Net.BCrypt.HashPassword("password"));
            _dbContext.Users.Add(_testUser);
            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task CreateAudioFileAsync_ShouldCreateSuccessfully()
        {
            var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, _testUser.Id.ToString())
            }));

            var dto = new AudioFileCreateDTO { Name = "Integration Audio" };
            var result = await _audioService.CreateAudioFileAsync(dto, userPrincipal);

            Assert.True(result.Success);
            Assert.Equal("Integration Audio", result.Data.Name);
        }

        [Fact]
        public async Task AssignAudioFileToSceneAsync_ShouldAssignSuccessfully()
        {
            var scene = new Scene { Name = "Integration Scene", User = _testUser };
            _dbContext.Scenes.Add(scene);

            var audioFile = new AudioFile { Name = "Unassigned Audio", User = _testUser };
            _dbContext.AudioFiles.Add(audioFile);

            await _dbContext.SaveChangesAsync();

            var assignDTO = new AudioFileAssignDTO
            {
                AudioFileId = audioFile.Id,
                SceneId = scene.Id
            };

            var result = await _audioService.AssignAudioFileToSceneAsync(assignDTO);

            Assert.True(result.Success);
            Assert.Equal(scene.Id, result.Data.SceneId);
        }
    }
}
