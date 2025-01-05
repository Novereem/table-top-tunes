using Shared.Enums;
using Shared.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class AudioFile : BaseEntity
    {
        public string Name { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public User User { get; set; }
        public Guid UserId { get; set; }
        public List<SceneAudioFile> SceneAudioFiles { get; set; } = new List<SceneAudioFile>();
        public DateTime CreatedAt { get; set; }
    }
}
