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
using System.ComponentModel.DataAnnotations;
using Shared.Factories;
using Shared.Interfaces.Services.Helpers;

namespace TTTBackend.Services
{
    public class SceneService : ISceneService
    {
        private readonly ISceneData _sceneData;
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserClaimsService _userClaimsService;
        private readonly ILogger<SceneService> _logger;
        private readonly ISceneServiceHelper _helper;

        public SceneService(
            ISceneData sceneData,
            IAuthenticationService authenticationService,
            IUserClaimsService userClaimsService,
            ILogger<SceneService> logger,
            ISceneServiceHelper sceneServiceHelper)
        {
            _sceneData = sceneData;
            _authenticationService = authenticationService;
            _userClaimsService = userClaimsService;
            _logger = logger;
            _helper = sceneServiceHelper;
        }

        public async Task<ServiceResult<SceneCreateResponseDTO>> CreateSceneAsync(SceneCreateDTO sceneDTO, ClaimsPrincipal user)
        {
            var userIdResult = _userClaimsService.GetUserIdFromClaims(user);
            if (!userIdResult.Success)
            {
                return userIdResult.ToFailureResult<SceneCreateResponseDTO>();
            }

            try
            {
                var userResult = await _authenticationService.GetUserByIdAsync(userIdResult.Data);
                if (!userResult.Success || userResult.Data == null)
                {
                    return userIdResult.ToFailureResult<SceneCreateResponseDTO>();
                }

                var validationResult = _helper.ValidateSceneCreateRequest(sceneDTO);
                if (!validationResult.Success)
                {
                    return userIdResult.ToFailureResult<SceneCreateResponseDTO>();
                }

                var createdSceneResult = await _helper.CreateSceneAsync(sceneDTO, userResult.Data);
                if (!createdSceneResult.Success)
                {
                    return userIdResult.ToFailureResult<SceneCreateResponseDTO>();
                }

                return ServiceResult<SceneCreateResponseDTO>.SuccessResult(
                    createdSceneResult.Data,
                    SuccessMessages.GetSuccessMessage(SuccessCode.SceneCreated),
                    SuccessMessagesUser.GetSuccessMessage(SuccessCodeUser.SceneCreated),
                    HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while creating the scene. Name: {Name}", sceneDTO.Name);
                return PredefinedFailures.GetFailure<SceneCreateResponseDTO>(ErrorCode.InternalServerError);
            }
        }

        public async Task<ServiceResult<SceneGetResponseDTO>> GetSceneByIdAsync(Guid id)
        {
            try
            {
                var sceneResult = await _helper.RetrieveSceneByIdAsync(id);
                if (!sceneResult.Success)
                {
                    return sceneResult.ToFailureResult<SceneGetResponseDTO>();
                }

                return ServiceResult<SceneGetResponseDTO>.SuccessResult(
                    sceneResult.Data.ToSceneGetResponseDTO(),
                    SuccessMessages.GetSuccessMessage(SuccessCode.Success),
                    httpStatusCode: HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving the scene. Scene ID: {SceneId}", id);
                return PredefinedFailures.GetFailure<SceneGetResponseDTO>(ErrorCode.InternalServerError);
            }
        }

        public async Task<ServiceResult<List<SceneListItemDTO>>> GetScenesListByUserIdAsync(ClaimsPrincipal user)
        {
            var userIdResult = _userClaimsService.GetUserIdFromClaims(user);
            if (!userIdResult.Success)
            {
                return userIdResult.ToFailureResult<List<SceneListItemDTO>>();
            }

            try
            {
                var scenesResult = await _helper.RetrieveScenesByUserIdAsync(userIdResult.Data);
                if (!scenesResult.Success)
                {
                    return userIdResult.ToFailureResult<List<SceneListItemDTO>>();
                }

                var sceneListItems = scenesResult.Data!.Select(scene => scene.ToSceneListItemDTO()).ToList();

                return ServiceResult<List<SceneListItemDTO>>.SuccessResult(sceneListItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving scenes for user: {UserId}", userIdResult.Data);
                return PredefinedFailures.GetFailure<List<SceneListItemDTO>>(ErrorCode.InternalServerError);
            }
        }
    }
}
