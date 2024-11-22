using Microsoft.EntityFrameworkCore;
using Shared.Interfaces.Services;
using Shared.Models.DTOs;
using Shared.Models;
using Shared.Models.Extensions;
using TTTBackend.Data;
using Shared.Interfaces.Data;

namespace TTTBackend.Services
{
    public class SceneService : ISceneService
    {
        private readonly ISceneData _sceneData;
        private readonly IUserService _userService;

        public SceneService(ISceneData sceneData, IUserService userService)
        {
            _sceneData = sceneData;
            _userService = userService;
        }

        public async Task<Scene> CreateSceneAsync(SceneCreateDTO sceneDTO, Guid userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);

            var newScene = sceneDTO.ToSceneFromSceneCreateDTO();
            newScene.User = user;

            return await _sceneData.CreateSceneAsync(newScene);
        }

        public async Task<Scene?> GetSceneByIdAsync(Guid id)
        {
            return await _sceneData.GetSceneByIdAsync(id);
        }
    }
}
