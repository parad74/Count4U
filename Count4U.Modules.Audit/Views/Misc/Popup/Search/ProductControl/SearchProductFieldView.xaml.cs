using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using Count4U.Modules.Audit.ViewModels.Misc.Popup.Search.ProductControl;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.Audit.Views.Misc.Popup.Search.ProductControl
{
    /// <summary>
    /// Interaction logic for SearchProductFieldView.xaml
    /// </summary>
    public partial class SearchProductFieldView : UserControl, INavigationAware
    {
        private readonly IRegionManager _regionManager;

        public SearchProductFieldView(
            SearchProductFieldViewModel viewModel,
            IRegionManager regionManager)
        {            
            InitializeComponent();

            this.DataContext = viewModel;
            _regionManager = regionManager;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            RegionManager.SetRegionName(sortControl, Common.RegionNames.Sort);

            _regionManager.RequestNavigate(Common.RegionNames.Sort, new Uri(Common.ViewNames.SortView, UriKind.Relative));
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            Utils.NavigateFromForInnerRegions(new List<ContentControl>
                                                  {
                                                        sortControl,                                                      
                                                  }, navigationContext);

            _regionManager.Regions.Remove(Common.RegionNames.Sort);
        }
    }
}
