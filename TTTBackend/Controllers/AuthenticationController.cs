using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Shared.Enums;
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
        var result = await _authService.RegisterUserAsync(registrationDTO);

        if (!result.Success)
        {
            return StatusCode(
                (int)(result.HttpStatusCode ?? HttpStatusCode.BadRequest),
                new { Message = result.ErrorMessage }
            );
        }

        return StatusCode(
            (int)(result.HttpStatusCode ?? HttpStatusCode.Created),
            new { Message = result.Data }
        );
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDTO loginModel)
    {
        var (result, token) = await _authService.ValidateUserAsync(loginModel);

        if (!result.Success)
        {
            return StatusCode(
                (int)(result.HttpStatusCode ?? HttpStatusCode.Unauthorized),
                new { Message = result.ErrorMessage }
            );
        }

        return StatusCode(
            (int)(result.HttpStatusCode ?? HttpStatusCode.OK),
            new { Token = token }
        );
    }
}