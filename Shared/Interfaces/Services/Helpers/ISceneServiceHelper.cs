using Shared.Models.Common;
using Shared.Models.DTOs;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Interfaces.Services.Helpers
{
    public interface ISceneServiceHelper
    {
        ServiceResult<object> ValidateSceneCreateRequest(SceneCreateDTO sceneDTO);
        Task<ServiceResult<SceneCreateResponseDTO>> CreateSceneAsync(SceneCreateDTO sceneDTO, User user);
        Task<ServiceResult<Scene>> RetrieveSceneByIdAsync(Guid sceneId);
        Task<ServiceResult<List<Scene>>> RetrieveScenesByUserIdAsync(Guid userId);
    }
}
