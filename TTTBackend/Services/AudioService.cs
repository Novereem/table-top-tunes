using Shared.Enums;
using Shared.Interfaces.Data;
using Shared.Interfaces.Services;
using Shared.Models.DTOs;
using Shared.Models.Sounds;
using Shared.Models;
using Shared.Models.Extensions;
using TTTBackend.Data;
using Shared.Constants;
using Shared.Models.Common;
using Shared.Interfaces.Services.CommonServices;
using System.Security.Claims;
using TTTBackend.Services.Helpers;
using Shared.Factories;
using Shared.Interfaces.Services.Helpers;
using System.Collections.Generic;

namespace TTTBackend.Services
{
    public class AudioService : IAudioService
    {
        private readonly IAudioData _audioData;
        private readonly ISceneServiceHelper _sceneServiceHelper;
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserClaimsService _userClaimsService;
        private readonly ILogger<AudioService> _logger;
        private readonly IAudioServiceHelper _helper;

        public AudioService(
            IAudioData audioData,
            ISceneServiceHelper sceneServiceHelper,
            IAuthenticationService authenticationService,
            IUserClaimsService userClaimsService,
            ILogger<AudioService> logger,
            IAudioServiceHelper audioServiceHelper)
        {
            _audioData = audioData;
            _sceneServiceHelper = sceneServiceHelper;
            _authenticationService = authenticationService;
            _userClaimsService = userClaimsService;
            _logger = logger;
            _helper = audioServiceHelper;
        }
        public async Task<ServiceResult<AudioFileResponseDTO>> CreateAudioFileAsync(AudioFileCreateDTO audioFileCreateDTO, ClaimsPrincipal user)
        {
            var userIdResult = _userClaimsService.GetUserIdFromClaims(user);
            if (!userIdResult.Success)
            {
                return userIdResult.ToFailureResult<AudioFileResponseDTO>();
            }

            var validationResult = _helper.ValidateAudioFileCreateRequest(audioFileCreateDTO);
            if (!validationResult.Success)
            {
                return validationResult.ToFailureResult<AudioFileResponseDTO>();
            }

            try
            {
                var retrievedUser = await _authenticationService.GetUserByIdAsync(userIdResult.Data);
                if (!retrievedUser.Success)
                {
                    return retrievedUser.ToFailureResult<AudioFileResponseDTO>();
                }

                var newAudioFile = audioFileCreateDTO.ToAudioFileFromCreateDTO(retrievedUser.Data!);
                await _audioData.SaveAudioFileAsync(newAudioFile);

                return ServiceResult<AudioFileResponseDTO>.SuccessResult(
                    newAudioFile.ToAudioFileResponseDTO(),
                    SuccessMessages.GetSuccessMessage(SuccessCode.Success),
                    SuccessMessagesUser.GetSuccessMessage(SuccessCodeUser.AudioCreated),
                    httpStatusCode: HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating audio file. Name: {Name}", audioFileCreateDTO.Name);
                return PredefinedFailures.GetFailure<AudioFileResponseDTO>(ErrorCode.InternalServerError);
            }
        }

        public async Task<ServiceResult<List<AudioFileListItemDTO>>> GetUserAudioFilesAsync(ClaimsPrincipal user)
        {
            var userIdResult = _userClaimsService.GetUserIdFromClaims(user);
            if (!userIdResult.Success)
            {
                return userIdResult.ToFailureResult<List<AudioFileListItemDTO>>();
            }

            try
            {
                var audiosResult = await _helper.RetrieveAudioFilesByUserIdAsync(userIdResult.Data);
                if (!audiosResult.Success)
                {
                    return userIdResult.ToFailureResult<List<AudioFileListItemDTO>>();
                }

                var audioFileListItems = audiosResult.Data!.Select(audio => audio.ToAudioFileListItemDTO()).ToList();

                return ServiceResult<List<AudioFileListItemDTO>>.SuccessResult(audioFileListItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving audio files for user: {UserId}", userIdResult.Data);
                return PredefinedFailures.GetFailure<List<AudioFileListItemDTO>>(ErrorCode.InternalServerError);
            }
            throw new NotImplementedException();
        }

        public async Task<ServiceResult<AudioFileResponseDTO>> AssignAudioFileToSceneAsync(AudioFileAssignDTO assignDTO)
        {
            var validationResult = _helper.ValidateAudioFileAssignRequest(assignDTO);
            if (!validationResult.Success)
            {
                return validationResult.ToFailureResult<AudioFileResponseDTO>();
            }
            try
            {
                var audioFileResult = await _helper.RetrieveAudioFileByIdAsync(assignDTO.AudioFileId);
                if (!audioFileResult.Success)
                {
                    return audioFileResult.ToFailureResult<AudioFileResponseDTO>();
                }

                var sceneResult = await _sceneServiceHelper.RetrieveSceneByIdAsync(assignDTO.SceneId);
                if (!sceneResult.Success)
                {
                    return sceneResult.ToFailureResult<AudioFileResponseDTO>();
                }

                var updatedAudioFile = assignDTO.ToAudioFileFromAssignDTO(audioFileResult.Data!, sceneResult.Data!);
                await _audioData.UpdateAudioFileAsync(updatedAudioFile);

                return ServiceResult<AudioFileResponseDTO>.SuccessResult(
                    updatedAudioFile.ToAudioFileResponseDTO(),
                    SuccessMessages.GetSuccessMessage(SuccessCode.Success),
                    httpStatusCode: HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while assigning audio file to scene. AudioFileId: {AudioFileId}, SceneId: {SceneId}", assignDTO.AudioFileId, assignDTO.SceneId);
                return PredefinedFailures.GetFailure<AudioFileResponseDTO>(ErrorCode.InternalServerError);
            }
        }
    }
}
