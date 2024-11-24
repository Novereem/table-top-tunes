using Shared.Interfaces.Services;
using Shared.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models.Extensions
{
    public static class SceneExtensions
    {
        public static Scene ToSceneFromSceneCreateDTO(this SceneCreateDTO dto)
        {
            return new Scene
            {
                Name = dto.Name
            };
        }
        public static SceneCreateResponseDTO ToSceneCreateResponseDTOFromScene(this Scene scene)
        {
            return new SceneCreateResponseDTO
			{
                Id = scene.Id,
                Name = scene.Name
            };
        }

        public static SceneGetResponseDTO? ToSceneGetResponseDTOFromScene(this Scene? scene)
        {
            if (scene == null) return null;

            return new SceneGetResponseDTO
            {
                Id = scene.Id,
                Name = scene.Name,
                SoundPresets = scene.SoundPresets,
                CreatedAt = scene.CreatedAt,
                AudioFiles = scene.AudioFiles.Select(audio => audio.ToAudioFileResponseDTO()).ToList(),
            };
        }

        public static SceneListItemDTO ToSceneListItemDTOFromScene(this Scene scene)
        {
            return new SceneListItemDTO
            {
                Id = scene.Id,
                Name = scene.Name,
                CreatedAt = scene.CreatedAt
            };
        }
    }
}