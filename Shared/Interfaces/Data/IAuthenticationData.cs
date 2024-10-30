using Shared.Models;
using Shared.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Interfaces.Data
{
	public interface IAuthenticationData
	{
		public Task RegisterUserAsync(User user);
		public Task<User?> GetUserByUsernameAsync(string username);
		public Task<User?> GetUserByEmailAsync(string email);
	}
}
