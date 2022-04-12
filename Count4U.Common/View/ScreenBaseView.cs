using System.Windows.Controls;
using Count4U.Model.Interface.Audit;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Common.View
{
    public abstract class ScreenBaseView : UserControl, INavigationAware 
    {
        protected readonly IRegionManager _regionManager;
        protected readonly IContextCBIRepository _contextCbiRepository;

        protected ScreenBaseView(
            IRegionManager regionManager,
            IContextCBIRepository contextCbiRepository)
        {
            _contextCbiRepository = contextCbiRepository;
            _regionManager = regionManager;
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