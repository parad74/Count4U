using System;
using System.Windows;
using System.Windows.Interactivity;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

namespace Count4U.Common.Helpers.Actions
{
    public class OpenFileDialogAction : TriggerAction<FrameworkElement>
    {
        #region Overrides of TriggerAction

        protected override void Invoke(object parameter)
        {
            InteractionRequestedEventArgs args = parameter as InteractionRequestedEventArgs;
            if (args == null) return;

            OpenFileDialogNotification notification = args.Context as OpenFileDialogNotification;

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = notification.DefaultExt;
            dlg.Filter = notification.Filter;
            dlg.CheckFileExists = true;
            dlg.CheckPathExists = true;
            dlg.Multiselect = false;
            if (!String.IsNullOrEmpty(notification.InitialDirectory))
                dlg.InitialDirectory = notification.InitialDirectory;

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog(Window.GetWindow(this.AssociatedObject));

            notification.IsOK = result == true;
            notification.FileName = dlg.FileName;
            
            args.Callback();
        }

        #endregion
    }
}