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
        public string FilePath { get; set; }
    }
}
