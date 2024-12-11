using Shared.Models.Common;
using Shared.Models.DTOs;
using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Interfaces.Services.Helpers
{
    public interface IAudioServiceHelper
    {
        ServiceResult<object> ValidateAudioFileCreateRequest(AudioFileCreateDTO createDTO);
        ServiceResult<object> ValidateAudioFileAssignRequest(AudioFileAssignDTO assignDTO);
        Task<ServiceResult<List<AudioFile>>> RetrieveAudioFilesByUserIdAsync(Guid userId);
        Task<ServiceResult<AudioFile>> RetrieveAudioFileByIdAsync(Guid audioFileId);
    }
}
