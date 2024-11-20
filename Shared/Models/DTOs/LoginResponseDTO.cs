﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models.DTOs
{
    public class LoginResponseDTO
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public string ErrorMessage { get; set; }
    }
}
