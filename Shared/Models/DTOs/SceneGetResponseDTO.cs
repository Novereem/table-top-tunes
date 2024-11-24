using Shared.Models.Sounds.Presets;
using Shared.Models.Sounds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models.DTOs
{
	public class SceneGetResponseDTO
	{
		public string Name { get; set; } = string.Empty;
		public List<MusicTrack> MusicTracks { get; set; } = new List<MusicTrack>();
		public List<AmbientSound> AmbientSounds { get; set; } = new List<AmbientSound>();
		public List<SoundEffect> SoundEffects { get; set; } = new List<SoundEffect>();
		public List<SoundPreset> SoundPresets { get; set; } = new List<SoundPreset>();
        public DateTime CreatedAt { get; set; }
    }
}
