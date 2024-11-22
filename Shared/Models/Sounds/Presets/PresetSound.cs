using Shared.Enums;
using Shared.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models.Sounds.Presets
{
    public class PresetSound : BaseEntity
    {
        public Guid SoundPresetId { get; set; }
        public SoundPreset SoundPreset { get; set; }
        public AudioFile Sound { get; set; }
        public SoundType SoundType { get; set; }
    }
}
