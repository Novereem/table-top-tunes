using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Interfaces.Services;
using Shared.Models.DTOs;
using System.Security.Claims;

namespace TTTBackend.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AudioController : ControllerBase
    {
        private readonly IAudioService _audioService;

        public AudioController(IAudioService audioService)
        {
            _audioService = audioService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAudioFile([FromBody] AudioFileCreateDTO audioFileCreateDTO)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized(new { Message = "User ID claim missing in token." });
                }

                var userId = Guid.Parse(userIdClaim.Value);

                var response = await _audioService.CreateAudioFileAsync(audioFileCreateDTO, userId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut("assign")]
        public async Task<IActionResult> AssignAudioFileToScene([FromBody] AudioFileAssignDTO assignDTO)
        {
            try
            {
                var response = await _audioService.AssignAudioFileToSceneAsync(assignDTO);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}