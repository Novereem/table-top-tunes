using Microsoft.EntityFrameworkCore;
using Shared.Interfaces.Data;
using Shared.Models;

namespace TTTBackend.Data
{
    public class SceneData : ISceneData
    {
        private readonly ApplicationDbContext _context;

        public SceneData(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Scene> CreateSceneAsync(Scene scene)
        {
            _context.Scenes.Add(scene);
            await _context.SaveChangesAsync();
            return scene;
        }

        public async Task<Scene?> GetSceneByIdAsync(Guid id)
        {
            return await _context.Scenes
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
		public async Task<List<Scene>> GetScenesByUserIdAsync(Guid userId)
		{
			return await _context.Scenes
				.Where(scene => scene.User.Id == userId)
                .OrderBy(scene => scene.CreatedAt)
                .ToListAsync();
		}
	}
}
