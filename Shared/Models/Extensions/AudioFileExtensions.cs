﻿using Shared.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models.Extensions
{
    public static class AudioFileExtensions
    {
        public static AudioFileResponseDTO ToAudioFileResponseDTO(this AudioFile audioFile)
        {
            return new AudioFileResponseDTO
            {
                Id = audioFile.Id,
                Name = audioFile.Name,
                FilePath = audioFile.FilePath,
                CreatedAt = audioFile.CreatedAt
            };
        }

        public static AudioFile ToAudioFile(this AudioFileCreateDTO audioFileCreateDTO, User user)
        {
            return new AudioFile
            {
                Name = audioFileCreateDTO.Name,
                FilePath = "/path/to/files/" + Guid.NewGuid() + ".mp3",
                User = user,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}