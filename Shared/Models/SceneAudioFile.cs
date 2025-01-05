using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class SceneAudioFile
    {
        public Guid SceneId { get; set; }
        public Scene Scene { get; set; }

        public Guid AudioFileId { get; set; }
        public AudioFile AudioFile { get; set; }

        public AudioType? Type { get; set; }
    }
}
