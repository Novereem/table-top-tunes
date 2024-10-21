﻿using Shared.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Interfaces.Data
{
	public interface IAuthenticationData
	{
		Task RegisterUserAsync(UserRegistrationDTO registrationDTO);
	}
}
