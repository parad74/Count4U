using System;
using System.Windows;
using System.Windows.Interactivity;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

namespace Count4U.Common.Helpers.Actions
{
    public class MessageBoxAction : TriggerAction<FrameworkElement>
    {
        #region Overrides of TriggerAction

        protected override void Invoke(object parameter)
        {
            InteractionRequestedEventArgs args = parameter as InteractionRequestedEventArgs;
            if (args == null) return;

            MessageBoxNotification notification = args.Context as MessageBoxNotification;
            if (notification == null)
                return;

            MessageBoxImage image = notification.Image;
            string title = String.IsNullOrEmpty(notification.Title) ? "Count4U" : notification.Title;

            var wnd = Window.GetWindow(this.AssociatedObject);

            if (wnd == null) return;

            UtilsMisc.ShowMessageBox(notification.Content.ToString(), MessageBoxButton.OK, image, notification.Settings, wnd);
        }

        #endregion
    }
}