using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Interfaces.Services;
using Shared.Models.DTOs;
using Shared.Enums;
using Shared.Models.Common.Extensions;

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
            return StatusCode(
                (int)(serviceResult.HttpStatusCode ?? HttpStatusCode.OK),
                serviceResult.ToApiResponse()
            );
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetScene(Guid id)
        {
            var serviceResult = await _sceneService.GetSceneByIdAsync(id);
            return StatusCode(
                (int)(serviceResult.HttpStatusCode ?? HttpStatusCode.OK),
                serviceResult.ToApiResponse()
            );
        }

        [HttpGet]
        public async Task<IActionResult> GetScenesList()
        {
            var serviceResult = await _sceneService.GetScenesListByUserIdAsync(User);
            return StatusCode(
                (int)(serviceResult.HttpStatusCode ?? HttpStatusCode.OK),
                serviceResult.ToApiResponse()
            );
        }
    }
}
