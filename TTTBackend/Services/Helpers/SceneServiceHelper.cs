using Shared.Constants;
using Shared.Enums;
using Shared.Interfaces.Data;
using Shared.Models.Common;
using Shared.Models.DTOs;
using Shared.Models;
using Shared.Models.Extensions;
using Shared.Factories;
using Shared.Interfaces.Services.Helpers;

namespace TTTBackend.Services.Helpers
{
    public class SceneServiceHelper : ISceneServiceHelper
    {
        private readonly ISceneData _sceneData;
        private readonly ILogger<SceneServiceHelper> _logger;

        public SceneServiceHelper(ISceneData sceneData, ILogger<SceneServiceHelper> logger)
        {
            _sceneData = sceneData;
            _logger = logger;
        }

        public ServiceResult<object> ValidateSceneCreateRequest(SceneCreateDTO sceneDTO)
        {
            if (string.IsNullOrWhiteSpace(sceneDTO.Name))
            {
                return PredefinedFailures.GetFailure<object>(ErrorCode.InvalidInput);
            }

            return ServiceResult<object>.SuccessResult();
        }

        public async Task<ServiceResult<SceneCreateResponseDTO>> CreateSceneAsync(SceneCreateDTO sceneDTO, User user)
        {
            try
            {
                var newScene = sceneDTO.ToSceneFromCreateDTO();
                newScene.User = user;
                newScene.CreatedAt = DateTime.UtcNow;

                var createdScene = await _sceneData.CreateSceneAsync(newScene);
                if (createdScene != null)
                {
                    return ServiceResult<SceneCreateResponseDTO>.SuccessResult(createdScene.ToCreateResponseDTO());
                }
                return PredefinedFailures.GetFailure<SceneCreateResponseDTO>(ErrorCode.CreationFailed);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating the scene. Name: {Name}", sceneDTO.Name);
                return PredefinedFailures.GetFailure<SceneCreateResponseDTO>(ErrorCode.InternalServerError);
            }
        }

        public async Task<ServiceResult<Scene>> RetrieveSceneByIdAsync(Guid sceneId)
        {
            var scene = await _sceneData.GetSceneByIdAsync(sceneId);
            if (scene == null)
            {
                _logger.LogWarning("Scene not found. SceneId: {SceneId}", sceneId);
                return PredefinedFailures.GetFailure<Scene>(ErrorCode.ResourceNotFound);
            }

            return ServiceResult<Scene>.SuccessResult(scene);
        }

        public async Task<ServiceResult<List<Scene>>> RetrieveScenesByUserIdAsync(Guid userId)
        {
            var scenes = await _sceneData.GetScenesByUserIdAsync(userId);

            if (scenes == null)
            {
                _logger.LogWarning("No scenes found for user. UserId: {UserId}", userId);
                scenes = new List<Scene>();
            }

            return ServiceResult<List<Scene>>.SuccessResult(scenes);
        }
    }
}
