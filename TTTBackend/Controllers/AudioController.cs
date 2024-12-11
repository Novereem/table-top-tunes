using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Enums;
using Shared.Interfaces.Services;
using Shared.Models.Common.Extensions;
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
            var serviceResult = await _audioService.CreateAudioFileAsync(audioFileCreateDTO, User);

            return StatusCode(
                (int)(serviceResult.HttpStatusCode ?? HttpStatusCode.OK),
                serviceResult.ToApiResponse()
            );
                
        }

        [HttpGet]
        public async Task<IActionResult> GetUserAudioFiles()
        {
            var serviceResult = await _audioService.GetUserAudioFilesAsync(User);

            return StatusCode(
                (int)(serviceResult.HttpStatusCode ?? HttpStatusCode.OK),
                serviceResult.ToApiResponse()
            );
        }

        [HttpPut("assign")]
        public async Task<IActionResult> AssignAudioFileToScene([FromBody] AudioFileAssignDTO assignDTO)
        {
            var serviceResult = await _audioService.AssignAudioFileToSceneAsync(assignDTO);
            return StatusCode(
                (int)(serviceResult.HttpStatusCode ?? HttpStatusCode.OK),
                serviceResult.ToApiResponse()
            );
        }
    }
}