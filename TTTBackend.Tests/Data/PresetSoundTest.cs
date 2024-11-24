using Shared.Enums;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TTTBackend.Tests.Factories;
using Microsoft.EntityFrameworkCore;

namespace TTTBackend.Tests.Data
{
    public class PresetSoundTest
    {
        [Fact]
        public void Test_AudioFileAndSceneRelationship()
        {
            // Arrange
            var context = TestDbContextFactory.CreateInMemoryDbContext();

            var scene = new Scene { Name = "Battle Scene" };
            var audioFile = new AudioFile { Name = "Battle Music", Scene = scene };

            context.Scenes.Add(scene);
            context.AudioFiles.Add(audioFile);
            context.SaveChanges();

            // Act
            var fetchedScene = context.Scenes
                .Include(s => s.AudioFiles)
                .FirstOrDefault(s => s.Name == "Battle Scene");

            var fetchedAudioFile = context.AudioFiles
                .Include(af => af.Scene)
                .FirstOrDefault(af => af.Name == "Battle Music");

            // Assert
            Assert.NotNull(fetchedScene);
            Assert.NotNull(fetchedAudioFile);
            Assert.Single(fetchedScene.AudioFiles);
            Assert.Equal("Battle Music", fetchedScene.AudioFiles.First().Name);
            Assert.Equal("Battle Scene", fetchedAudioFile.Scene.Name);
        }
    }
}