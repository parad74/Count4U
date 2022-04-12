using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Count4U.Common.Helpers;
using Count4U.Model.Interface.Audit;
using Count4U.Planogram.ViewModel;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Planogram.View
{
    /// <summary>
    /// Interaction logic for PlanBasementView.xaml
    /// </summary>
    public partial class PlanBasementView : UserControl, INavigationAware
    {
        private readonly IRegionManager _regionManager;
        private readonly IContextCBIRepository _contextCbiRepository;

        public PlanBasementView(
            PlanBasementViewModel viewModel,
            IRegionManager regionManager,
            IContextCBIRepository contextCbiRepository)
        {
            _contextCbiRepository = contextCbiRepository;
            _regionManager = regionManager;
            InitializeComponent();

            this.DataContext = viewModel;

            RegionManager.SetRegionName(contentCanvas, Common.RegionNames.PlanogramCanvas);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            RegionManager.SetRegionName(backForward, Common.RegionNames.PlanBasementBackForward);

            Utils.SetCurrentAuditConfig(navigationContext, this._contextCbiRepository);            

            UtilsNavigate.ApplicationStripNavigateFromNavigationContext(navigationContext, this._regionManager);
            UtilsNavigate.BackForwardNavigate(this._regionManager, Common.RegionNames.PlanBasementBackForward);

            UriQuery query = Utils.UriQueryFromNavigationContext(navigationContext);
            _regionManager.RequestNavigate(Common.RegionNames.PlanogramCanvas, new Uri(Common.ViewNames.PlanCanvasView + query, UriKind.Relative));
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
                                                      contentCanvas,
                                                  }, navigationContext);

            this._regionManager.Regions.Remove(Common.RegionNames.PlanBasementBackForward);
            this._regionManager.Regions.Remove(Common.RegionNames.PlanogramCanvas);            
        }
    }
}
