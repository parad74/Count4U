using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Modules.ContextCBI.ViewModels.Misc.Popup.MdiFilter.Items
{
    public class MdiFilterItemViewModel : NotificationObject
    {
        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                RaisePropertyChanged(()=>IsChecked);
            }
        }

        public string Text { get; set; }
        public MdiFilterItem State { get; set; }
    }
}