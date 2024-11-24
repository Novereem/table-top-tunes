using Shared.Interfaces.Services;
using Shared.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public static SceneGetResponseDTO ToSceneGetResponseDTOFromScene(this Scene scene)
        {
            return new SceneGetResponseDTO
            {
                Name = scene.Name,
                MusicTracks = scene.MusicTracks,
                AmbientSounds = scene.AmbientSounds,
                SoundEffects = scene.SoundEffects,
                SoundPresets = scene.SoundPresets,
                CreatedAt = scene.CreatedAt
            };
        }
	}
}