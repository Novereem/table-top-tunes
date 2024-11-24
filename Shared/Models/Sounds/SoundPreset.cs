using Shared.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models.Sounds
{
    public class SoundPreset : BaseEntity
    {
        public string Name { get; set; }
        public Scene Scene { get; set; }
        public Guid SceneId { get; set; }
        public List<PresetSound> PresetSounds { get; set; } = new List<PresetSound>();
    }
}
