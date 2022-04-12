using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

namespace Count4U.Common.Helpers.Actions
{
    public class SaveFileDialogNotification : Notification
    {
        public string DefaultExt { get; set; }
        public string Filter { get; set; }

        public bool IsOK { get; set; }
        public string FileName { get; set; }
    }
}