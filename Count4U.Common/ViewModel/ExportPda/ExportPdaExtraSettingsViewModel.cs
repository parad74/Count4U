using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

namespace Count4U.Common.ViewModel.ExportPda
{
    public class ExportPdaExtraSettingsViewModel : NotificationObject, INavigationAware
    {
        private bool _isAutoPrint;

        public ExportPdaExtraSettingsViewModel()
        {
            
        }

        public bool IsAutoPrint
        {
            get { return _isAutoPrint; }
            set
            {
                _isAutoPrint = value;
                RaisePropertyChanged(() => IsAutoPrint);
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {

        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }
    }
}