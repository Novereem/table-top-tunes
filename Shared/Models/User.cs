using Shared.Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class User : BaseEntity
	{
		public string Username { get; set; }
		public string PasswordHash { get; set; }

		public User(string username, string passwordHash)
		{
			Username = username;
			PasswordHash = passwordHash;
		}
	}
}
