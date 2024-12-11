namespace TTTFrontend.Services
{
    public class NotificationService
    {
        public event Action<string, string> OnShow;
        public event Action OnHide;

        public void ShowNotification(string message, string type = "error")
        {
            OnShow?.Invoke(message, type);
        }

        public void HideNotification()
        {
            OnHide?.Invoke();
        }
    }
}
