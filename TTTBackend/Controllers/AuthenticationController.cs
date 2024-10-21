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

	private string GenerateJwtToken(string username, string secretKey)
	{
		var claims = new[]
		{
			new Claim(JwtRegisteredClaimNames.Sub, username),
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
		};

		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var token = new JwtSecurityToken(
			issuer: Environment.GetEnvironmentVariable("JWT_ISSUER"),
			audience: Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
			claims: claims,
			expires: DateTime.Now.AddMinutes(30), // Token expires in 30 minutes
			signingCredentials: creds
		);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}
}