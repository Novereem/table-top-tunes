using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Constants
{
    public static class SuccessMessages
    {
        private static readonly Dictionary<SuccessCode, string> SuccessMessagesMap = new()
        {
            //Authentication
            { SuccessCode.Register, "Successful Registration." },
            { SuccessCode.Login, "Successful Login." },

            //Scenes
            { SuccessCode.SceneCreated, "Succesful Scene Creation" },

            //Audio
            { SuccessCode.AudioCreated, "Succesful Audio Creation" },

            //Common
            { SuccessCode.Success, "Executed Successfully" },
        };

        public static string GetSuccessMessage(SuccessCode code)
        {
            return SuccessMessagesMap.TryGetValue(code, out var message) ? message : "An unknown success occurred.";
        }
    }
}
