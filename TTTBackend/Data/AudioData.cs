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
    }
}
