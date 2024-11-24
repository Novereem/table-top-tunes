using Shared.Enums;
using Shared.Models.DTOs;

public class SelectedSceneService
{
    public event Action OnSceneChanged;

    private SceneGetResponseDTO _selectedScene;
    public SceneGetResponseDTO SelectedScene
    {
        get => _selectedScene;
        set
        {
            _selectedScene = value;
            FilterAudioFiles();
            NotifySceneChanged();
        }
    }

    public List<AudioFileResponseDTO> MusicTracks { get; private set; } = new List<AudioFileResponseDTO>();
    public List<AudioFileResponseDTO> AmbientSounds { get; private set; } = new List<AudioFileResponseDTO>();
    public List<AudioFileResponseDTO> ActionSounds { get; private set; } = new List<AudioFileResponseDTO>();

    private void FilterAudioFiles()
    {
        if (_selectedScene?.AudioFiles != null)
        {
            MusicTracks = _selectedScene.AudioFiles
                .Where(audio => audio.Type == AudioType.MusicTrack)
                .ToList();
            AmbientSounds = _selectedScene.AudioFiles
                .Where(audio => audio.Type == AudioType.AmbientSound)
                .ToList();
            ActionSounds = _selectedScene.AudioFiles
                .Where(audio => audio.Type == AudioType.SoundEffect)
                .ToList();
        }
        else
        {
            MusicTracks = new List<AudioFileResponseDTO>();
            AmbientSounds = new List<AudioFileResponseDTO>();
            ActionSounds = new List<AudioFileResponseDTO>();
        }
    }

    private void NotifySceneChanged()
    {
        OnSceneChanged?.Invoke();
    }
}