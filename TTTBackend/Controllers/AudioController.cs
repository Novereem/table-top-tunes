using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
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
            var result = await _audioService.CreateAudioFileAsync(audioFileCreateDTO, User);

            if (!result.Success)
            {
                return StatusCode((int)(result.HttpStatusCode ?? HttpStatusCode.BadRequest), new { Message = result.ErrorMessage });
            }

            return StatusCode((int)(result.HttpStatusCode ?? HttpStatusCode.Created), result.Data);
        }

        [HttpPut("assign")]
        public async Task<IActionResult> AssignAudioFileToScene([FromBody] AudioFileAssignDTO assignDTO)
        {
            var result = await _audioService.AssignAudioFileToSceneAsync(assignDTO);

            if (!result.Success)
            {
                return StatusCode((int)(result.HttpStatusCode ?? HttpStatusCode.BadRequest), new { Message = result.ErrorMessage });
            }

            return Ok(result.Data);
        }
    }
}