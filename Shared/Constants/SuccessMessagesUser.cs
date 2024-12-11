using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Constants
{
    public static class SuccessMessagesUser
    {
        private static readonly Dictionary<SuccessCodeUser, string> SuccessMessagesMap = new()
        {
            //Authentication
            { SuccessCodeUser.Register, "Registered successfully!" },
            { SuccessCodeUser.Login, "Logged in successfully!" },

            //Scenes
            { SuccessCodeUser.SceneCreated, "Scene created successfully!" },

            //Audio
            { SuccessCodeUser.AudioCreated, "Audio created successfully!" },

            //Common
            { SuccessCodeUser.Success, "Action executed successfully!" },
        };

        public static string GetSuccessMessage(SuccessCodeUser code)
        {
            return SuccessMessagesMap.TryGetValue(code, out var message) ? message : "An unknown success occurred, please try again later";
        }
    }
}
