using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.Audit.ViewModels.Reports
{
    public class ReportsViewModel : INavigationAware, IRegionMemberLifetime
    {
        public void OnNavigatedTo(NavigationContext navigationContext)
        {

        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        public bool KeepAlive
        {
            get { return true; }
        }
    }
}