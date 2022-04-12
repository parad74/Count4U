using System.Windows;
using Count4U.Common.UserSettings;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

namespace Count4U.Common.Helpers.Actions
{
    public class MessageBoxNotification : Notification
    {
        public MessageBoxImage Image { get; set; }
        public IUserSettingsManager Settings { get; set; }
    }
}