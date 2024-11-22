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
                // The User property is left null here, as it will be assigned later in the service layer
            };
        }
    }
}
