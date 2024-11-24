using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models.DTOs
{
    public class AudioFileResponseDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string FilePath { get; set; }
        public DateTime CreatedAt { get; set; }
        public AudioType? Type { get; set; }
        public Guid? SceneId { get; set; }
    }
}
