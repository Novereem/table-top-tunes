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

namespace TTTBackend.Services
{
    public class AudioService : IAudioService
    {
        private readonly IAudioData _audioData;
        private readonly IUserData _userData;
        private readonly ISceneData _sceneData;
        private readonly IUserClaimsService _userClaimsService;
        private readonly ILogger<AudioService> _logger;
        private readonly AudioServiceHelper _helper;

        public AudioService(IAudioData audioData, IUserData userData, ISceneData sceneData, IUserClaimsService userClaimsService, ILogger<AudioService> logger)
        {
            _audioData = audioData;
            _userData = userData;
            _sceneData = sceneData;
            _userClaimsService = userClaimsService;
            _logger = logger;
            _helper = new AudioServiceHelper(audioData, userData, sceneData, logger);
        }

        public async Task<ServiceResult<AudioFileResponseDTO>> CreateAudioFileAsync(AudioFileCreateDTO audioFileCreateDTO, ClaimsPrincipal user)
        {
            var userIdResult = _userClaimsService.GetUserIdFromClaims(user);
            if (!userIdResult.Success)
            {
                _logger.LogWarning("Failed to retrieve user ID. Error: {ErrorMessage}", userIdResult.ErrorMessage);
                return ServiceResult<AudioFileResponseDTO>.Failure(userIdResult.ErrorMessage, userIdResult.HttpStatusCode);
            }

            var validationResult = _helper.ValidateAudioFileCreateRequest(audioFileCreateDTO);
            if (!validationResult.Success)
            {
                return ServiceResult<AudioFileResponseDTO>.Failure(validationResult.ErrorMessage, HttpStatusCode.BadRequest);
            }

            try
            {
                var retrievedUser = await _helper.RetrieveUserByIdAsync(userIdResult.Data);
                if (!retrievedUser.Success)
                {
                    return ServiceResult<AudioFileResponseDTO>.Failure(retrievedUser.ErrorMessage, HttpStatusCode.NotFound);
                }

                var newAudioFile = audioFileCreateDTO.ToAudioFileFromCreateDTO(retrievedUser.Data!);
                await _audioData.SaveAudioFileAsync(newAudioFile);

                return ServiceResult<AudioFileResponseDTO>.SuccessResult(newAudioFile.ToAudioFileResponseDTO(), HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating audio file. Name: {Name}", audioFileCreateDTO.Name);
                return ServiceResult<AudioFileResponseDTO>.Failure(ErrorMessages.GetErrorMessage(ErrorCode.UnknownError), HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ServiceResult<AudioFileResponseDTO>> AssignAudioFileToSceneAsync(AudioFileAssignDTO assignDTO)
        {
            var validationResult = _helper.ValidateAudioFileAssignRequest(assignDTO);
            if (!validationResult.Success)
            {
                return ServiceResult<AudioFileResponseDTO>.Failure(validationResult.ErrorMessage, HttpStatusCode.BadRequest);
            }

            try
            {
                var audioFileResult = await _helper.RetrieveAudioFileByIdAsync(assignDTO.AudioFileId);
                if (!audioFileResult.Success)
                {
                    return ServiceResult<AudioFileResponseDTO>.Failure(audioFileResult.ErrorMessage, HttpStatusCode.NotFound);
                }

                var sceneResult = await _helper.RetrieveSceneByIdAsync(assignDTO.SceneId);
                if (!sceneResult.Success)
                {
                    return ServiceResult<AudioFileResponseDTO>.Failure(sceneResult.ErrorMessage, HttpStatusCode.NotFound);
                }

                var updatedAudioFile = assignDTO.ToAudioFileFromAssignDTO(audioFileResult.Data!, sceneResult.Data!);
                await _audioData.UpdateAudioFileAsync(updatedAudioFile);

                return ServiceResult<AudioFileResponseDTO>.SuccessResult(updatedAudioFile.ToAudioFileResponseDTO(), HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while assigning audio file to scene. AudioFileId: {AudioFileId}, SceneId: {SceneId}", assignDTO.AudioFileId, assignDTO.SceneId);
                return ServiceResult<AudioFileResponseDTO>.Failure(ErrorMessages.GetErrorMessage(ErrorCode.UnknownError), HttpStatusCode.InternalServerError);
            }
        }
    }
}
