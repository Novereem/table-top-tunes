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
            { SuccessCode.Register, "Successful Registration." },
            { SuccessCode.Login, "Successful Login." },
        };

        public static string GetSuccessMessage(SuccessCode code)
        {
            return SuccessMessagesMap.TryGetValue(code, out var message) ? message : "An unknown success occurred.";
        }
    }
}
