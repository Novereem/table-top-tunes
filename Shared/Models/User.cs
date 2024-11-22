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
		public string Email { get; set; }
		public string PasswordHash { get; set; }

        public List<Scene> Scenes { get; set; } = new List<Scene>();

        public User() { }
        public User(string username, string email, string passwordHash)
		{
			Username = username;
			Email = email;
			PasswordHash = passwordHash;
		}
    }
}
