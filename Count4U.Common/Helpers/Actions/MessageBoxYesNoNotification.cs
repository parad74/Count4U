using System.Windows;
using Count4U.Common.UserSettings;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

namespace Count4U.Common.Helpers.Actions
{
    public class MessageBoxYesNoNotification : Notification
    {
        public bool IsYes { get; set; }
        public MessageBoxImage? Image { get; set; }
        public IUserSettingsManager Settings { get; set; }
    }
}