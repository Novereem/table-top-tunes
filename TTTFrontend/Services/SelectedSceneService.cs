using Shared.Models.DTOs;

namespace TTTFrontend.Services
{
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
                NotifySceneChanged();
            }
        }

        private void NotifySceneChanged()
        {
            OnSceneChanged?.Invoke();
        }
    }
}
