using Shared.Models.DTOs;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Interfaces.Services
{
    public interface ISceneService
    {
        Task<Scene> CreateSceneAsync(SceneCreateDTO sceneDTO, Guid userId);
        Task<Scene?> GetSceneByIdAsync(Guid id);
    }
}
