﻿using Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Interfaces.Data
{
    public interface IAudioData
    {
        Task SaveAudioFileAsync(AudioFile audioFile);
        Task<AudioFile> GetAudioFileByIdAsync(Guid audioFileId);
        Task<List<AudioFile>> GetAudioFilesByUserIdAsync(Guid userId);
        Task UpdateAudioFileAsync(AudioFile audioFile);
    }
}
