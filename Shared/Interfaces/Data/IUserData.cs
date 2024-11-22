using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Interfaces.Data
{
    public interface IUserData
    {
        Task<User?> GetUserByIdAsync(Guid userId);
    }
}
