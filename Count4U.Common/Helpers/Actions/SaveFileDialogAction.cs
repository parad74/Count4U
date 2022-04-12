using System;
using System.Windows;
using System.Windows.Interactivity;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

namespace Count4U.Common.Helpers.Actions
{
    public class SaveFileDialogAction : TriggerAction<FrameworkElement>
    {
        protected override void Invoke(object parameter)
        {
            InteractionRequestedEventArgs args = parameter as InteractionRequestedEventArgs;
            if (args == null) return;

            SaveFileDialogNotification notification = args.Context as SaveFileDialogNotification;
            if (notification == null) return;

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.DefaultExt = notification.DefaultExt;
            dlg.Filter = notification.Filter;                        

            // Show open file dialog box
            Nullable<bool> result = dlg.ShowDialog(Window.GetWindow(this.AssociatedObject));

            notification.IsOK = result == true;
            notification.FileName = dlg.FileName;

            args.Callback();
        }
    }
}