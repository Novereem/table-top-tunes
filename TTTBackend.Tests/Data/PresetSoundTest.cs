using Shared.Enums;
using Shared.Models.Sounds.Presets;
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
        public void Test_SoundPresetAndPresetSoundRelationship()
        {
            // Arrange
            var context = TestDbContextFactory.CreateInMemoryDbContext();

            var soundPreset = new SoundPreset { Name = "Battle Preset" };
            var audioFile = new AudioFile { Name = "Wind Effect", FilePath = "path/to/wind.mp3" };
            var presetSound = new PresetSound { SoundPreset = soundPreset, Sound = audioFile, SoundType = SoundType.AmbientSound };

            soundPreset.PresetSounds.Add(presetSound);
            context.SoundPresets.Add(soundPreset);
            context.SaveChanges();

            // Act
            var fetchedPreset = context.SoundPresets.Include(sp => sp.PresetSounds).ThenInclude(ps => ps.Sound).FirstOrDefault();

            // Assert
            Assert.NotNull(fetchedPreset);
            Assert.Single(fetchedPreset.PresetSounds);
            Assert.Equal("Wind Effect", fetchedPreset.PresetSounds.First().Sound.Name);
        }
    }
}
