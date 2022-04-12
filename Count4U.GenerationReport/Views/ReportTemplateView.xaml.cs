using System.Collections.Generic;
using System.Windows.Controls;
using Count4U.Common.Helpers;
using Count4U.Model.Interface.Audit;
using Count4U.Report.ViewModels;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Report.Views
{
    /// <summary>
    /// Interaction logic for ReportTemplateView.xaml
    /// </summary>
    public partial class ReportTemplateView : UserControl, INavigationAware, IRegionMemberLifetime 
    {
        private readonly IRegionManager _regionManager;
        private readonly IContextCBIRepository _contextCbiRepository;

        public ReportTemplateView(
            ReportTemplateViewModel viewModel, 
            IRegionManager regionManager, 
            IContextCBIRepository contextCbiRepository)
        {            
            InitializeComponent();

            _regionManager = regionManager;
            DataContext = viewModel;
            _contextCbiRepository = contextCbiRepository;
        }

        public bool KeepAlive { get { return false; } }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            Utils.SetCurrentAuditConfig(navigationContext, _contextCbiRepository);
            UtilsNavigate.BackForwardNavigate(_regionManager, Common.RegionNames.ReportTemplateBackForward);
            UtilsNavigate.ApplicationStripNavigateFromNavigationContext(navigationContext, _regionManager);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            Utils.NavigateFromForInnerRegions(new List<ContentControl>
                                                  {
                                                        backForward,         
                                                  }, navigationContext);

            _regionManager.Regions.Remove(Common.RegionNames.ReportTemplateBackForward);
        }
    }
}
