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
using TTTBackend.Services.Helpers;

namespace TTTBackend.Services
{
    public class SceneService : ISceneService
    {
        private readonly ISceneData _sceneData;
        private readonly IUserService _userService;
        private readonly IUserClaimsService _userClaimsService;
        private readonly ILogger<SceneService> _logger;
        private readonly SceneServiceHelper _helper;

        public SceneService(ISceneData sceneData, IUserService userService, IUserClaimsService userClaimsService, ILogger<SceneService> logger)
        {
            _sceneData = sceneData;
            _userService = userService;
            _userClaimsService = userClaimsService;
            _logger = logger;
            _helper = new SceneServiceHelper(sceneData, logger);
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

                var validationResult = _helper.ValidateSceneCreateRequest(sceneDTO);
                if (!validationResult.Success)
                {
                    return ServiceResult<SceneCreateResponseDTO>.Failure(validationResult.ErrorMessage, HttpStatusCode.BadRequest);
                }

                var createdSceneResult = await _helper.CreateSceneAsync(sceneDTO, userResult.Data);
                if (!createdSceneResult.Success)
                {
                    return ServiceResult<SceneCreateResponseDTO>.Failure(createdSceneResult.ErrorMessage, createdSceneResult.HttpStatusCode);
                }

                return ServiceResult<SceneCreateResponseDTO>.SuccessResult(createdSceneResult.Data, HttpStatusCode.Created);
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
                var sceneResult = await _helper.RetrieveSceneByIdAsync(id);
                if (!sceneResult.Success)
                {
                    return ServiceResult<SceneGetResponseDTO>.Failure(sceneResult.ErrorMessage, sceneResult.HttpStatusCode);
                }

                return ServiceResult<SceneGetResponseDTO>.SuccessResult(sceneResult.Data.ToSceneGetResponseDTOFromScene());
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
                var scenesResult = await _helper.RetrieveScenesByUserIdAsync(userIdResult.Data);
                if (!scenesResult.Success)
                {
                    return ServiceResult<List<SceneListItemDTO>>.Failure(scenesResult.ErrorMessage);
                }

                var sceneListItems = scenesResult.Data!.Select(scene => scene.ToSceneListItemDTOFromScene()).ToList();

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
