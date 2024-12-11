using Shared.Models.DTOs;

namespace TTTFrontend.Services
{
    public class SelectedUserSoundsService
    {
        public event Action OnSoundsChanged;

        public List<AudioFileListItemDTO> UserSounds { get; private set; } = new();

        public async Task SetUserSoundsAsync(List<AudioFileListItemDTO> sounds)
        {
            UserSounds = sounds;
            NotifySoundsChanged();
        }

        private void NotifySoundsChanged()
        {
            OnSoundsChanged?.Invoke();
        }
    }
}
