using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Interfaces.Services
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(Guid userId);
    }
}
