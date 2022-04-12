using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Count4U.Common.Helpers;
using Count4U.Modules.Audit.ViewModels;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.Audit.Views.Misc.Popup.Search.IturControl
{
    /// <summary>
    /// Interaction logic for SearchIturFieldView.xaml
    /// </summary>
    public partial class SearchIturFieldView : UserControl, INavigationAware
    {
        private readonly IRegionManager _regionManager;

        public SearchIturFieldView(
            SearchIturFieldViewModel viewModel,
            IRegionManager regionManager)
        {
            _regionManager = regionManager;
            InitializeComponent();

            this.Loaded += SearchIturFieldView_Loaded;
            this.DataContext = viewModel;
        }

        void SearchIturFieldView_Loaded(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() => txtNumber.Focus()), DispatcherPriority.Background);
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
