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

	/*
	[HttpPost("login")]
	public IActionResult Login([FromBody] UserLoginDTO loginModel)
	{
		// Test the login credentials (hardcoded for simplicity)
		if (loginModel.Username != "testuser" || loginModel.Password != "password123")
		{
			return Unauthorized("Invalid credentials");
		}

		// Retrieve the secret key directly from environment variables
		var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
		Console.WriteLine($"Secret Key in Controller: {secretKey}"); // Debugging purpose
		if (string.IsNullOrEmpty(secretKey))
		{
			throw new InvalidOperationException("JWT secret key is not set in the .env file.");
		}

		// Generate JWT Token
		var token = GenerateJwtToken(loginModel.Username, secretKey);

		// Return the generated token
		return Ok(new { Token = token });
	}
	*/

	[HttpPost("register")]
	public async Task<IActionResult> Register([FromBody] UserRegistrationDTO registrationDTO)
	{
		// Call the service method and receive a tuple result
		var (success, errorMessage) = await _authService.RegisterUserAsync(registrationDTO);

		// Check if registration succeeded
		if (success)
		{
			return Ok(new { message = "User registered successfully" });
		}
		else
		{
			// Return a bad request with the specific error message
			return BadRequest(new { error = errorMessage });
		}
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