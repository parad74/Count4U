using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interactivity;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

namespace Count4U.Common.Helpers.Actions
{
    public class OpenFolderDialogAction : TriggerAction<FrameworkElement>
    {
        protected override void Invoke(object parameter)
        {
            InteractionRequestedEventArgs args = parameter as InteractionRequestedEventArgs;
            if (args == null) return;

            OpenFolderDialogNotification notification = args.Context as OpenFolderDialogNotification;

            if (notification == null)
                return;

            var dlg = new FolderBrowserDialog();
            dlg.RootFolder = Environment.SpecialFolder.MyComputer;
            if (!String.IsNullOrWhiteSpace(notification.FolderPath) && Directory.Exists(notification.FolderPath))
            {
                dlg.SelectedPath = notification.FolderPath;
            }
            DialogResult result = dlg.ShowDialog(GetIWin32Window(this.AssociatedObject));

            if (result == DialogResult.OK)
            {
                notification.IsOk = true;
                notification.FolderPath = dlg.SelectedPath;

                args.Callback();
            }
        }

        public static System.Windows.Forms.IWin32Window GetIWin32Window(System.Windows.Media.Visual visual)
        {
            var source = System.Windows.PresentationSource.FromVisual(visual) as System.Windows.Interop.HwndSource;
            System.Windows.Forms.IWin32Window win = new OldWindow(source.Handle);
            return win;
        }

        private class OldWindow : System.Windows.Forms.IWin32Window
        {
            private readonly System.IntPtr _handle;
            public OldWindow(System.IntPtr handle)
            {
                this._handle = handle;
            }

            #region IWin32Window Members
            System.IntPtr System.Windows.Forms.IWin32Window.Handle
            {
                get { return this._handle; }
            }
            #endregion
        }

    }
}