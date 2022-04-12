using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Interactivity;
using Count4U.Common.Behaviours;
using Count4U.Common.Helpers;
using Count4U.Modules.ContextCBI.ViewModels.Misc.Popup.Search.InventorControl;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.ContextCBI.Views.Misc.Popup.Search.InventorControl
{
    /// <summary>
    /// Interaction logic for SearchViewCustomerField.xaml
    /// </summary>
    public partial class SearchInventorFieldView : UserControl, INavigationAware
    {
        private readonly IRegionManager _regionManager;

        public SearchInventorFieldView(
            SearchInventorFieldViewModel viewModel,
            IRegionManager regionManager)
        {          
            InitializeComponent();

            this.DataContext = viewModel;

            _regionManager = regionManager;

            Interaction.GetBehaviors(filterCustomer).Add(new TextChangedDelayedBehavior() { IsTimerEnabled = true });
            Interaction.GetBehaviors(filterBranch).Add(new TextChangedDelayedBehavior() { IsTimerEnabled = true });
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
