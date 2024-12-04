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

namespace TTTBackend.Tests.Services.Integration
{
    public class AudioServiceIntegrationTests
    {
        private readonly AudioService _audioService;
        private readonly ApplicationDbContext _dbContext;

        public AudioServiceIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "AudioServiceTestDB")
                .Options;

            _dbContext = new ApplicationDbContext(options);

            var audioData = new AudioData(_dbContext);
            var sceneData = new SceneData(_dbContext);
            var userData = new UserData(_dbContext);

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });

            var userClaimsService = new UserClaimsService();
            var logger = loggerFactory.CreateLogger<AudioService>();

            _audioService = new AudioService(audioData, userData, sceneData, userClaimsService, logger);
        }

        [Fact]
        public async Task CreateAudioFileAsync_ShouldCreateAudioFileSuccessfully()
        {
            var user = new User("testuser", "test@example.com", "hashedpassword");
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "test"));

            var audioFileDTO = new AudioFileCreateDTO { Name = "Test Audio" };
            var result = await _audioService.CreateAudioFileAsync(audioFileDTO, claimsPrincipal);

            Assert.True(result.Success);
            Assert.Equal(audioFileDTO.Name, result.Data.Name);
        }

        [Fact]
        public async Task AssignAudioFileToSceneAsync_ShouldAssignSuccessfully()
        {
            var user = new User("testuser", "test@example.com", "hashedpassword");
            var scene = new Scene { Name = "Test Scene", User = user };
            var audioFile = new AudioFile { Name = "Test Audio", User = user };

            _dbContext.Users.Add(user);
            _dbContext.Scenes.Add(scene);
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
