using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Count4U.Common.Helpers;
using Count4U.Modules.ContextCBI.ViewModels.Misc.Popup.Search.BranchControl;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.ContextCBI.Views.Misc.Popup.Search.BranchControl
{
    /// <summary>
    /// Interaction logic for SearchCustomerFieldView.xaml
    /// </summary>
    public partial class SearchBranchFieldView : UserControl, INavigationAware
    {
        private readonly IRegionManager _regionManager;

        public SearchBranchFieldView(
            SearchBranchFieldViewModel viewModel,
            IRegionManager regionManager)
        {            
            InitializeComponent();

            _regionManager = regionManager;

            this.DataContext = viewModel;
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
