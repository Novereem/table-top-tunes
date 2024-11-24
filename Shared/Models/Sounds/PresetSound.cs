using Shared.Enums;
using Shared.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models.Sounds
{
    public class PresetSound : BaseEntity
    {
        public SoundPreset SoundPreset { get; set; }
        public Guid SoundPresetId { get; set; }
        public AudioFile Sound { get; set; }
        public Guid? SoundId { get; set; }
    }
}
