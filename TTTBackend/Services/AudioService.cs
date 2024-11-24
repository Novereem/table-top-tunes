using Shared.Enums;
using Shared.Interfaces.Data;
using Shared.Interfaces.Services;
using Shared.Models.DTOs;
using Shared.Models.Sounds;
using Shared.Models;
using Shared.Models.Extensions;
using TTTBackend.Data;

namespace TTTBackend.Services
{
    public class AudioService : IAudioService
    {
        private readonly IAudioData _audioData;
        private readonly IUserData _userData;
        private readonly ISceneData _sceneData;

        public AudioService(IAudioData audioData, IUserData userData, ISceneData sceneData)
        {
            _audioData = audioData;
            _userData = userData;
            _sceneData = sceneData;
        }

        public async Task<AudioFileResponseDTO> CreateAudioFileAsync(AudioFileCreateDTO audioFileCreateDTO, Guid userId)
        {
            if (string.IsNullOrWhiteSpace(audioFileCreateDTO.Name))
            {
                throw new ArgumentException("Audio file name cannot be empty.");
            }

            var user = await _userData.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            var newAudioFile = audioFileCreateDTO.ToAudioFileFromCreateDTO(user);
            await _audioData.SaveAudioFileAsync(newAudioFile);
            return newAudioFile.ToAudioFileResponseDTO();
        }

        public async Task<AudioFileResponseDTO> AssignAudioFileToSceneAsync(AudioFileAssignDTO assignDTO)
        {
            if (assignDTO.AudioFileId == Guid.Empty || assignDTO.SceneId == Guid.Empty)
            {
                throw new ArgumentException("AudioFileId and SceneId must not be empty.");
            }

            var audioFile = await _audioData.GetAudioFileByIdAsync(assignDTO.AudioFileId);
            if (audioFile == null)
            {
                throw new Exception("AudioFile not found.");
            }

            var scene = await _sceneData.GetSceneByIdAsync(assignDTO.SceneId);
            if (scene == null)
            {
                throw new Exception("Scene not found.");
            }

            audioFile = assignDTO.ToAudioFileFromAssignDTO(audioFile, scene);
            await _audioData.UpdateAudioFileAsync(audioFile);
            return audioFile.ToAudioFileResponseDTO();
        }
    }
}
