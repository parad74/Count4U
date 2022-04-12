using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Count4U.Common.Controls;
using Count4U.Common.Helpers;
using Count4U.Modules.Audit.ViewModels;
using Count4U.Modules.Audit.ViewModels.Misc.Popup.Search.IturControl;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.Audit.Views.Misc.Popup.Search.IturControl
{
    /// <summary>
    /// Interaction logic for SearchIturAdvancedFieldView.xaml
    /// </summary>
    public partial class SearchIturAdvancedFieldView : UserControl, INavigationAware
    {
        private readonly IRegionManager _regionManager;

        public SearchIturAdvancedFieldView(
            SearchIturAdvancedFieldViewModel viewModel,
            IRegionManager regionManager)
        {            
            InitializeComponent();

            _regionManager = regionManager;

            this.DataContext = viewModel;
            viewModel.View = this;

            this.Loaded += IturFilterView_Loaded;

            txtIturFilter.KeyUp += Txt_KeyUp;
            txtName.KeyUp += Txt_KeyUp;
            txtBarcode.KeyUp += Txt_KeyUp;
            txtMakat.KeyUp += Txt_KeyUp;
        }

        void Txt_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SearchIturAdvancedFieldViewModel viewModel = this.DataContext as SearchIturAdvancedFieldViewModel;
                if (viewModel != null)
                {
                  
                }
            }
        }

        void IturFilterView_Loaded(object sender, RoutedEventArgs e)
        {
            txtIturFilter.Focus();
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
