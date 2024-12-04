using Shared.Models;
using Shared.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Interfaces.Services
{
    public interface IUserService
    {
        Task<ServiceResult<User>> GetUserByIdAsync(Guid userId);
    }
}
