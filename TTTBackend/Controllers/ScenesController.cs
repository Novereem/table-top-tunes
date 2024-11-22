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
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized(new { Message = "UserId claim missing in token." });
                }

                var userId = Guid.Parse(userIdClaim.Value);
                var newScene = await _sceneService.CreateSceneAsync(sceneDTO, userId);

                return CreatedAtAction(nameof(GetScene), new { id = newScene.Id }, newScene);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetScene(Guid id)
        {
            var scene = await _sceneService.GetSceneByIdAsync(id);

            if (scene == null)
                return NotFound();

            return Ok(scene);
        }
    }
}
