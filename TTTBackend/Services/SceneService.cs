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

        public async Task<SceneCreateResponseDTO> CreateSceneAsync(SceneCreateDTO sceneDTO, Guid userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);

            var newScene = sceneDTO.ToSceneFromSceneCreateDTO();
            newScene.User = user;
            newScene.CreatedAt = DateTime.UtcNow;

            var newCreatedScene = await _sceneData.CreateSceneAsync(newScene);

			return newCreatedScene.ToSceneCreateResponseDTOFromScene();
        }

        public async Task<SceneGetResponseDTO?> GetSceneByIdAsync(Guid id)
        {
            var scene = await _sceneData.GetSceneByIdAsync(id);
            return scene?.ToSceneGetResponseDTOFromScene();
        }

        public async Task<List<SceneListItemDTO>> GetScenesListByUserIdAsync(Guid userId)
        {
            var scenes = await _sceneData.GetScenesByUserIdAsync(userId);

            return scenes.Select(scene => scene.ToSceneListItemDTOFromScene()).ToList();
        }
    }
}
