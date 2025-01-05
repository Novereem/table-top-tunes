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

        public async Task UpdateAudioFileAsync(AudioFile audioFile)
        {
            foreach (var sceneAudioFile in audioFile.SceneAudioFiles)
            {
                var existing = await _context.Set<SceneAudioFile>()
                    .FirstOrDefaultAsync(sa => sa.SceneId == sceneAudioFile.SceneId
                                               && sa.AudioFileId == sceneAudioFile.AudioFileId
                                               && sa.Type == sceneAudioFile.Type);

                if (existing == null)
                {
                    _context.Set<SceneAudioFile>().Add(sceneAudioFile);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
