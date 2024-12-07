using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
using Shared.Interfaces.Services;
using Shared.Models.DTOs;
using Shared.Models.Common.Extensions;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
	private readonly IAuthenticationService _authService;

	public AuthenticationController(IAuthenticationService authService)
	{
		_authService = authService;
	}

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserRegistrationDTO registrationDTO)
    {
        var serviceResult = await _authService.RegisterUserAsync(registrationDTO);
        return StatusCode(
            (int)(serviceResult.HttpStatusCode ?? HttpStatusCode.OK),
            serviceResult.ToApiResponse()
        );
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDTO loginDTO)
    {
        var serviceResult = await _authService.ValidateUserAsync(loginDTO);
        return StatusCode(
            (int)(serviceResult.HttpStatusCode ?? HttpStatusCode.OK),
            serviceResult.ToApiResponse()
        );
    }
}