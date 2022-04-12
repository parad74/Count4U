using System.Windows;
using System.Windows.Interactivity;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

namespace Count4U.Common.Helpers.Actions
{
    public class MessageBoxYesNoAction : TriggerAction<FrameworkElement>
    {
        #region Overrides of TriggerAction

        protected override void Invoke(object parameter)
        {
            InteractionRequestedEventArgs args = parameter as InteractionRequestedEventArgs;
            if (args == null) return;

            MessageBoxYesNoNotification notification = args.Context as MessageBoxYesNoNotification;
            if (notification == null)
                return;

            var wnd = Window.GetWindow(this.AssociatedObject);

            if (wnd == null) return;

            MessageBoxResult result = UtilsMisc.ShowMessageBox(notification.Content.ToString(), MessageBoxButton.YesNo, notification.Image ?? MessageBoxImage.Information, notification.Settings, wnd);

            notification.IsYes = result == MessageBoxResult.Yes;
            args.Callback();
        }

        #endregion
    }
}