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
using System.Windows.Threading;
using Count4U.Common.Helpers;
using Count4U.Modules.Audit.ViewModels.Misc.Popup.Search.SectionControl;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.Audit.Views.Misc.Popup.Search.SectionControl
{
    /// <summary>
    /// Interaction logic for SearchSectionFieldView.xaml
    /// </summary>
    public partial class SearchSectionFieldView : UserControl, INavigationAware
    {
        private readonly IRegionManager _regionManager;

        public SearchSectionFieldView(
            SearchSectionFieldViewModel viewModel,
            IRegionManager regionManager)
        {            
            InitializeComponent();

            _regionManager = regionManager;

            this.DataContext = viewModel;

            this.Loaded += SearchSectionFieldView_Loaded;
        }

        void SearchSectionFieldView_Loaded(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() => txtCode.Focus()), DispatcherPriority.Background);
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
