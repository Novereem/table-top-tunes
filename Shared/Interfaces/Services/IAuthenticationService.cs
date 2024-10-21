﻿using Shared.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Interfaces.Services
{
    public interface IAuthenticationService
    {
		public Task<(bool Success, string ErrorMessage)> RegisterUserAsync(UserRegistrationDTO registrationDTO);

	}
}