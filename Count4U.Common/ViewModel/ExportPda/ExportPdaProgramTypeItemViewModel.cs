using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Common.ViewModel.ExportPda
{
    public class ExportPdaProgramTypeItemViewModel : NotificationObject
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}