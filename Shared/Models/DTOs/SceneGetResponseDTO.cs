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
        public List<AudioFile> AudioFiles { get; set; } = new List<AudioFile>();
        public List<SoundPreset> SoundPresets { get; set; } = new List<SoundPreset>();
        public DateTime CreatedAt { get; set; }
    }
}