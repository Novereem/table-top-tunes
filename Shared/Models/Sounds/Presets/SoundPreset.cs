using Shared.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models.Sounds.Presets
{
    public class SoundPreset : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public List<PresetSound> PresetSounds { get; set; } = new List<PresetSound>();
        public Scene Scene { get; set; }
    }
}
