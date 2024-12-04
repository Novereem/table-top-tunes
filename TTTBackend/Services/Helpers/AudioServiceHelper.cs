using Shared.Constants;
using Shared.Enums;
using Shared.Interfaces.Data;
using Shared.Models.Common;
using Shared.Models.DTOs;
using Shared.Models;

namespace TTTBackend.Services.Helpers
{
    public class AudioServiceHelper
    {
        private readonly IAudioData _audioData;
        private readonly IUserData _userData;
        private readonly ISceneData _sceneData;
        private readonly ILogger _logger;

        public AudioServiceHelper(IAudioData audioData, IUserData userData, ISceneData sceneData, ILogger logger)
        {
            _audioData = audioData;
            _userData = userData;
            _sceneData = sceneData;
            _logger = logger;
        }

        public ServiceResult<bool> ValidateAudioFileCreateRequest(AudioFileCreateDTO createDTO)
        {
            if (string.IsNullOrWhiteSpace(createDTO.Name))
            {
                _logger.LogWarning("Audio file creation failed due to empty name.");
                return ServiceResult<bool>.Failure(ErrorMessages.GetErrorMessage(ErrorCode.InvalidInput));
            }

            return ServiceResult<bool>.SuccessResult();
        }

        public ServiceResult<bool> ValidateAudioFileAssignRequest(AudioFileAssignDTO assignDTO)
        {
            if (assignDTO.AudioFileId == Guid.Empty || assignDTO.SceneId == Guid.Empty)
            {
                _logger.LogWarning("Audio file assignment failed due to empty IDs.");
                return ServiceResult<bool>.Failure(ErrorMessages.GetErrorMessage(ErrorCode.InvalidInput));
            }

            return ServiceResult<bool>.SuccessResult();
        }

        public async Task<ServiceResult<User>> RetrieveUserByIdAsync(Guid userId)
        {
            var user = await _userData.GetUserByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User not found. User ID: {UserId}", userId);
                return ServiceResult<User>.Failure(ErrorMessages.GetErrorMessage(ErrorCode.UserNotFound));
            }

            return ServiceResult<User>.SuccessResult(user);
        }

        public async Task<ServiceResult<AudioFile>> RetrieveAudioFileByIdAsync(Guid audioFileId)
        {
            var audioFile = await _audioData.GetAudioFileByIdAsync(audioFileId);
            if (audioFile == null)
            {
                _logger.LogWarning("Audio file not found. AudioFileId: {AudioFileId}", audioFileId);
                return ServiceResult<AudioFile>.Failure(ErrorMessages.GetErrorMessage(ErrorCode.AudioFileNotFound));
            }

            return ServiceResult<AudioFile>.SuccessResult(audioFile);
        }

        public async Task<ServiceResult<Scene>> RetrieveSceneByIdAsync(Guid sceneId)
        {
            var scene = await _sceneData.GetSceneByIdAsync(sceneId);
            if (scene == null)
            {
                _logger.LogWarning("Scene not found. SceneId: {SceneId}", sceneId);
                return ServiceResult<Scene>.Failure(ErrorMessages.GetErrorMessage(ErrorCode.SceneNotFound));
            }

            return ServiceResult<Scene>.SuccessResult(scene);
        }
    }
}
