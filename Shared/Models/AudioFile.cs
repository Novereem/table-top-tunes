using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
	public class AudioFile
	{
		public Guid Id { get; set; }
		public string Name { get; set; }

		public AudioFile(string name)
		{
			Id = Guid.NewGuid();
			Name = name;
		}
	}
}
