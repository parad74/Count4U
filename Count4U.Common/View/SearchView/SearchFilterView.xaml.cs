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
using Count4U.Common.ViewModel.SearchFilter;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Common.View.SearchView
{
    /// <summary>
    /// Interaction logic for SearchFilterView.xaml
    /// </summary>
    public partial class SearchFilterView : UserControl, INavigationAware, IRegionMemberLifetime
    {
        public SearchFilterView(SearchFilterViewModel viewModel)
        {
            InitializeComponent();

            this.DataContext = viewModel;

            viewModel.BtnFilter = btnFilter;
            viewModel.BtnSearch = btnSearch;
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

        public bool KeepAlive { get { return false; } }
    }
}
