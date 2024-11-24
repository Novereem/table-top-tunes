using Shared.Models.Common;
using Shared.Models.Sounds;
using Shared.Models.Sounds.Presets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class Scene : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public User User { get; set; }
        public List<MusicTrack> MusicTracks { get; set; } = new List<MusicTrack>();
        public List<AmbientSound> AmbientSounds { get; set; } = new List<AmbientSound>();
        public List<SoundEffect> SoundEffects { get; set; } = new List<SoundEffect>();
        public List<SoundPreset> SoundPresets { get; set; } = new List<SoundPreset>();
        public DateTime CreatedAt { get; set; }
    }
}
