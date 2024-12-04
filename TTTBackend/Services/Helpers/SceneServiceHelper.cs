using Shared.Constants;
using Shared.Enums;
using Shared.Interfaces.Data;
using Shared.Models.Common;
using Shared.Models.DTOs;
using Shared.Models;
using Shared.Models.Extensions;

namespace TTTBackend.Services.Helpers
{
    public class SceneServiceHelper
    {
        private readonly ISceneData _sceneData;
        private readonly ILogger _logger;

        public SceneServiceHelper(ISceneData sceneData, ILogger logger)
        {
            _sceneData = sceneData;
            _logger = logger;
        }

        public ServiceResult<bool> ValidateSceneCreateRequest(SceneCreateDTO sceneDTO)
        {
            if (string.IsNullOrWhiteSpace(sceneDTO.Name))
            {
                _logger.LogWarning("Scene creation failed due to empty name.");
                return ServiceResult<bool>.Failure(ErrorMessages.GetErrorMessage(ErrorCode.InvalidInput));
            }

            return ServiceResult<bool>.SuccessResult();
        }

        public async Task<ServiceResult<SceneCreateResponseDTO>> CreateSceneAsync(SceneCreateDTO sceneDTO, User user)
        {
            try
            {
                var newScene = sceneDTO.ToSceneFromSceneCreateDTO();
                newScene.User = user;
                newScene.CreatedAt = DateTime.UtcNow;

                var createdScene = await _sceneData.CreateSceneAsync(newScene);
                if (createdScene == null)
                {
                    _logger.LogError("Failed to create scene. SceneData returned null for Name: {Name}", sceneDTO.Name);
                    return ServiceResult<SceneCreateResponseDTO>.Failure(ErrorMessages.GetErrorMessage(ErrorCode.UnknownError), HttpStatusCode.InternalServerError);
                }

                return ServiceResult<SceneCreateResponseDTO>.SuccessResult(createdScene.ToSceneCreateResponseDTOFromScene());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating the scene. Name: {Name}", sceneDTO.Name);
                return ServiceResult<SceneCreateResponseDTO>.Failure(ErrorMessages.GetErrorMessage(ErrorCode.UnknownError), HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ServiceResult<Scene>> RetrieveSceneByIdAsync(Guid sceneId)
        {
            var scene = await _sceneData.GetSceneByIdAsync(sceneId);
            if (scene == null)
            {
                _logger.LogWarning("Scene not found. SceneId: {SceneId}", sceneId);
                return ServiceResult<Scene>.Failure(ErrorMessages.GetErrorMessage(ErrorCode.ResourceNotFound));
            }

            return ServiceResult<Scene>.SuccessResult(scene);
        }

        public async Task<ServiceResult<List<Scene>>> RetrieveScenesByUserIdAsync(Guid userId)
        {
            var scenes = await _sceneData.GetScenesByUserIdAsync(userId);
            if (scenes == null || !scenes.Any())
            {
                _logger.LogWarning("No scenes found for user. UserId: {UserId}", userId);
                return ServiceResult<List<Scene>>.Failure(ErrorMessages.GetErrorMessage(ErrorCode.ResourceNotFound));
            }

            return ServiceResult<List<Scene>>.SuccessResult(scenes);
        }
    }
}
