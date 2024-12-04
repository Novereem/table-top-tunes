using Shared.Models.DTOs;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Models.Common;
using System.Security.Claims;

namespace Shared.Interfaces.Services
{
    public interface ISceneService
    {
        Task<ServiceResult<SceneCreateResponseDTO>> CreateSceneAsync(SceneCreateDTO sceneDTO, ClaimsPrincipal user);
        Task<ServiceResult<SceneGetResponseDTO>> GetSceneByIdAsync(Guid id);
        Task<ServiceResult<List<SceneListItemDTO>>> GetScenesListByUserIdAsync(ClaimsPrincipal user);
    }
}
