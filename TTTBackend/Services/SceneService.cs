using Microsoft.EntityFrameworkCore;
using Shared.Interfaces.Services;
using Shared.Models.DTOs;
using Shared.Models;
using Shared.Models.Extensions;
using TTTBackend.Data;
using Shared.Interfaces.Data;
using Shared.Enums;
using Shared.Models.Common;
using System.Security.Claims;
using Shared.Interfaces.Services.CommonServices;
using TTTBackend.Services.CommonServices;
using Shared.Constants;

namespace TTTBackend.Services
{
    public class SceneService : ISceneService
    {
        private readonly ISceneData _sceneData;
        private readonly IUserService _userService;
        private readonly IUserClaimsService _userClaimsService;
        private readonly ILogger<AuthenticationService> _logger;

        public SceneService(ISceneData sceneData, IUserService userService, IUserClaimsService userClaimsService, ILogger<AuthenticationService> logger)
        {
            _sceneData = sceneData;
            _userService = userService;
            _userClaimsService = userClaimsService;
            _logger = logger;
        }

        public async Task<ServiceResult<SceneCreateResponseDTO>> CreateSceneAsync(SceneCreateDTO sceneDTO, ClaimsPrincipal user)
        {
            var userIdResult = _userClaimsService.GetUserIdFromClaims(user);
            if (!userIdResult.Success)
            {
                return ServiceResult<SceneCreateResponseDTO>.Failure(userIdResult.ErrorMessage, userIdResult.HttpStatusCode);
            }

            try
            {
                var userResult = await _userService.GetUserByIdAsync(userIdResult.Data);
                if (!userResult.Success || userResult.Data == null)
                {
                    return ServiceResult<SceneCreateResponseDTO>.Failure(userResult.ErrorMessage, userResult.HttpStatusCode);
                }

                var newScene = sceneDTO.ToSceneFromSceneCreateDTO();
                newScene.User = userResult.Data;
                newScene.CreatedAt = DateTime.UtcNow;

                var createdScene = await _sceneData.CreateSceneAsync(newScene);

                if (createdScene == null)
                {
                    _logger.LogError("Failed to create scene. SceneData returned null for Name: {Name}", sceneDTO.Name);
                    return ServiceResult<SceneCreateResponseDTO>.Failure(ErrorMessages.GetErrorMessage(ErrorCode.UnknownError), HttpStatusCode.InternalServerError);
                }

                return ServiceResult<SceneCreateResponseDTO>.SuccessResult(createdScene.ToSceneCreateResponseDTOFromScene(), HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while creating the scene. Name: {Name}", sceneDTO.Name);
                return ServiceResult<SceneCreateResponseDTO>.Failure(ErrorMessages.GetErrorMessage(ErrorCode.UnknownError), HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ServiceResult<SceneGetResponseDTO>> GetSceneByIdAsync(Guid id)
        {
            try
            {
                var scene = await _sceneData.GetSceneByIdAsync(id);

                if (scene == null)
                {
                    return ServiceResult<SceneGetResponseDTO>.Failure(ErrorMessages.GetErrorMessage(ErrorCode.ResourceNotFound), HttpStatusCode.NotFound);
                }

                return ServiceResult<SceneGetResponseDTO>.SuccessResult(scene.ToSceneGetResponseDTOFromScene());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving the scene. Scene ID: {SceneId}", id);
                return ServiceResult<SceneGetResponseDTO>.Failure(ErrorMessages.GetErrorMessage(ErrorCode.UnknownError), HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ServiceResult<List<SceneListItemDTO>>> GetScenesListByUserIdAsync(ClaimsPrincipal user)
        {
            var userIdResult = _userClaimsService.GetUserIdFromClaims(user);
            if (!userIdResult.Success)
            {
                return ServiceResult<List<SceneListItemDTO>>.Failure(userIdResult.ErrorMessage, userIdResult.HttpStatusCode);
            }

            try
            {
                var scenes = await _sceneData.GetScenesByUserIdAsync(userIdResult.Data);

                var sceneListItems = scenes.Select(scene => scene.ToSceneListItemDTOFromScene()).ToList();

                return ServiceResult<List<SceneListItemDTO>>.SuccessResult(sceneListItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving scenes for user: {UserId}", userIdResult.Data);
                return ServiceResult<List<SceneListItemDTO>>.Failure(ErrorMessages.GetErrorMessage(ErrorCode.UnknownError), HttpStatusCode.InternalServerError);
            }
        }
    }
}
