using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

namespace Count4U.Common.Helpers.Actions
{
    public class OpenFolderDialogNotification : Notification
    {        
        public bool IsOk { get; set; }
        public string FolderPath { get; set; }

    }
}