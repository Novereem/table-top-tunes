using Shared.Constants;
using Shared.Enums;
using Shared.Interfaces.Data;
using Shared.Models.Common;
using Shared.Models.DTOs;
using Shared.Models;
using Shared.Factories;
using Shared.Interfaces.Services.Helpers;
using TTTBackend.Data;

namespace TTTBackend.Services.Helpers
{
    public class AudioServiceHelper : IAudioServiceHelper
    {
        private readonly IAudioData _audioData;
        private readonly ILogger<AudioServiceHelper> _logger;

        public AudioServiceHelper(IAudioData audioData, ILogger<AudioServiceHelper> logger)
        {
            _audioData = audioData;
            _logger = logger;
        }

        public ServiceResult<object> ValidateAudioFileCreateRequest(AudioFileCreateDTO createDTO)
        {
            if (string.IsNullOrWhiteSpace(createDTO.Name))
            {
                return PredefinedFailures.GetFailure<object>(ErrorCode.InvalidInput);
            }

            return ServiceResult<object>.SuccessResult();
        }

        public ServiceResult<object> ValidateAudioFileAssignRequest(AudioFileAssignDTO assignDTO)
        {
            if (assignDTO.AudioFileId == Guid.Empty || assignDTO.SceneId == Guid.Empty)
            {
                return PredefinedFailures.GetFailure<object>(ErrorCode.InternalServerError);
            }

            return ServiceResult<object>.SuccessResult();
        }

        public async Task<ServiceResult<List<AudioFile>>> RetrieveAudioFilesByUserIdAsync(Guid userId)
        {
            var audioFiles = await _audioData.GetAudioFilesByUserIdAsync(userId);

            if (audioFiles == null)
            {
                audioFiles = new List<AudioFile>();
            }

            return ServiceResult<List<AudioFile>>.SuccessResult(audioFiles);
        }

        public async Task<ServiceResult<AudioFile>> RetrieveAudioFileByIdAsync(Guid audioFileId)
        {
            var audioFile = await _audioData.GetAudioFileByIdAsync(audioFileId);
            if (audioFile == null)
            {
                return PredefinedFailures.GetFailure<AudioFile>(ErrorCode.AudioFileNotFound);
            }

            return ServiceResult<AudioFile>.SuccessResult(audioFile);
        }
    }
}
