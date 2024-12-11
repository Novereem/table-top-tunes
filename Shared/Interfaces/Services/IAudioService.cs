using Shared.Models.DTOs;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Models.Common;
using System.Security.Claims;

namespace Shared.Interfaces.Services
{
    public interface IAudioService
    {
        Task<ServiceResult<AudioFileResponseDTO>> CreateAudioFileAsync(AudioFileCreateDTO audioFileCreateDTO, ClaimsPrincipal user);
        Task<ServiceResult<List<AudioFileListItemDTO>>> GetUserAudioFilesAsync(ClaimsPrincipal user);
        Task<ServiceResult<AudioFileResponseDTO>> AssignAudioFileToSceneAsync(AudioFileAssignDTO assignDTO);
    }
}
