using Shared.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models.Sounds
{
    public class AmbientSound : BaseEntity
    {
        public AudioFile AudioFile { get; set; } = new AudioFile();
        public Scene Scene { get; set; }
    }
}
