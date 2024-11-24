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
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<AudioFileResponseDTO> AudioFiles { get; set; } = new List<AudioFileResponseDTO>();
        public List<SoundPreset> SoundPresets { get; set; } = new List<SoundPreset>();
        public DateTime CreatedAt { get; set; }
    }
}