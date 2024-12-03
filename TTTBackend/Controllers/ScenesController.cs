using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Interfaces.Services;
using Shared.Models;
using Shared.Models.DTOs;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Text.Json;
using Shared.Enums;

namespace TTTBackend.Controllers
{
	[Authorize]
	[ApiController]
    [Route("api/[controller]")]
    public class ScenesController : ControllerBase
    {
        private readonly ISceneService _sceneService;

        public ScenesController(ISceneService sceneService)
        {
            _sceneService = sceneService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateScene([FromBody] SceneCreateDTO sceneDTO)
        {
            var serviceResult = await _sceneService.CreateSceneAsync(sceneDTO, User);

            if (!serviceResult.Success)
            {
                return StatusCode((int)(serviceResult.HttpStatusCode ?? HttpStatusCode.BadRequest), new { Message = serviceResult.ErrorMessage });
            }

            return CreatedAtAction(
                nameof(GetScene),
                new { id = serviceResult.Data!.Id },
                serviceResult.Data
            );
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetScene(Guid id)
        {
            var serviceResult = await _sceneService.GetSceneByIdAsync(id);

            if (!serviceResult.Success)
            {
                return StatusCode((int)(serviceResult.HttpStatusCode ?? HttpStatusCode.BadRequest), new { Message = serviceResult.ErrorMessage });
            }

            return Ok(serviceResult.Data);
        }

        [HttpGet]
        public async Task<IActionResult> GetScenesList()
        {
            var serviceResult = await _sceneService.GetScenesListByUserIdAsync(User);

            if (!serviceResult.Success)
            {
                return StatusCode((int)(serviceResult.HttpStatusCode ?? HttpStatusCode.BadRequest), new { Message = serviceResult.ErrorMessage });
            }

            return Ok(serviceResult.Data);
        }
    }
}
