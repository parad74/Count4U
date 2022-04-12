using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Count4U.Common.Helpers;
using Count4U.Modules.ContextCBI.ViewModels.Misc.Popup.Search.CustomerControl;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.ContextCBI.Views.Misc.Popup.Search.CustomerControl
{
    /// <summary>
    /// Interaction logic for SearchCustomerFieldView.xaml
    /// </summary>
    public partial class SearchCustomerFieldView : UserControl, INavigationAware
    {
        private readonly IRegionManager _regionManager;

        public SearchCustomerFieldView(
            SearchCustomerFieldViewModel viewModel,
            IRegionManager regionManager)
        {
            _regionManager = regionManager;
            InitializeComponent();
            
            this.DataContext = viewModel;

            this.Loaded += SearchCustomerFieldView_Loaded;
        }

        void SearchCustomerFieldView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() => txtName.Focus()), DispatcherPriority.Background);
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
