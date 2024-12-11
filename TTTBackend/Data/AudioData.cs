using Microsoft.EntityFrameworkCore;
using Shared.Interfaces.Data;
using Shared.Models;

namespace TTTBackend.Data
{
    public class AudioData : IAudioData
    {
        private readonly ApplicationDbContext _context;

        public AudioData(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SaveAudioFileAsync(AudioFile audioFile)
        {
            _context.AudioFiles.Add(audioFile);
            await _context.SaveChangesAsync();
        }

        public async Task<AudioFile> GetAudioFileByIdAsync(Guid audioFileId)
        {
            return await _context.AudioFiles.FirstOrDefaultAsync(af => af.Id == audioFileId);
        }

        public async Task<List<AudioFile>> GetAudioFilesByUserIdAsync(Guid userId)
        {
            return await _context.AudioFiles
                .Where(audio => audio.User.Id == userId)
                .OrderBy(audio => audio.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Scene>> GetScenesByUserIdAsync(Guid userId)
        {
            return await _context.Scenes
                .Where(scene => scene.User.Id == userId)
                .OrderBy(scene => scene.CreatedAt)
                .Include(scene => scene.AudioFiles)
                .ToListAsync();
        }

        public async Task UpdateAudioFileAsync(AudioFile audioFile)
        {
            _context.AudioFiles.Update(audioFile);
            await _context.SaveChangesAsync();
        }
    }
}
