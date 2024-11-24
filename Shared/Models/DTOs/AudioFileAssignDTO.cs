using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models.DTOs
{
    public class AudioFileAssignDTO
    {
        public Guid AudioFileId { get; set; }
        public Guid SceneId { get; set; }
        public AudioType Type { get; set; }
    }
}
