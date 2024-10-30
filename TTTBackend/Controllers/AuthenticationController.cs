using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Shared.Interfaces.Services;
using Shared.Models.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
		var (success, errorMessage) = await _authService.RegisterUserAsync(registrationDTO);

		if (success)
		{
			return Ok(new { message = "User registered successfully" });
		}
		else
		{
			return BadRequest(new { error = errorMessage });
		}
	}

	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] UserLoginDTO loginModel)
	{
		var result = await _authService.ValidateUserAsync(loginModel);

		if (!result.Success)
		{
			return Unauthorized(result.ErrorMessage);
		}

		var token = _authService.GenerateJwtToken(result.Username);
		return Ok(new { Token = token });
	}
}