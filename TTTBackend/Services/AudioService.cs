using Shared.Enums;
using Shared.Interfaces.Data;
using Shared.Interfaces.Services;
using Shared.Models.DTOs;
using Shared.Models.Sounds;
using Shared.Models;
using Shared.Models.Extensions;

namespace TTTBackend.Services
{
    public class AudioService : IAudioService
    {
        private readonly IAudioData _audioData;
        private readonly IUserData _userData;

        public AudioService(IAudioData audioData, IUserData userData)
        {
            _audioData = audioData;
            _userData = userData;
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

            var newAudioFile = audioFileCreateDTO.ToAudioFile(user);
            await _audioData.SaveAudioFileAsync(newAudioFile);
            return newAudioFile.ToAudioFileResponseDTO();
        }
    }
}
