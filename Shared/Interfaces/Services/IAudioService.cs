using Shared.Models.DTOs;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Interfaces.Services
{
    public interface IAudioService
    {
        Task<AudioFileResponseDTO> CreateAudioFileAsync(AudioFileCreateDTO audioFileCreateDTO, Guid userId);
        Task<AudioFileResponseDTO> AssignAudioFileToSceneAsync(AudioFileAssignDTO assignDTO);
    }
}
