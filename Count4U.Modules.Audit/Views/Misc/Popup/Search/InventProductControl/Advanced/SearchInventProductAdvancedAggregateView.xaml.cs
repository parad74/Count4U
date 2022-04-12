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
using Count4U.Modules.Audit.ViewModels.Misc.Popup.Search.InventProductControl;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.Audit.Views.Misc.Popup.Search.InventProductControl
{
    /// <summary>
    /// Interaction logic for SearchInventProductAdvancedView.xaml
    /// </summary>
    public partial class SearchInventProductAdvancedAggregateView : UserControl, INavigationAware
    {
        private readonly IRegionManager _regionManager;

		public SearchInventProductAdvancedAggregateView(
            SearchInventProductAdvancedViewModel viewModel,
            IRegionManager regionManager)
        {
            _regionManager = regionManager;
            InitializeComponent();
			viewModel.IsSimple = false;
            this.DataContext = viewModel;
            viewModel.View = this;

            RegionManager.SetRegionName(content, Common.RegionNames.SearchGridInventProductAdvancedGround);
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
            Utils.NavigateFromForInnerRegions(new List<ContentControl>() { content }, navigationContext);

            _regionManager.Regions.Remove(Common.RegionNames.SearchGridInventProductAdvancedGround);
        }
    }
}
