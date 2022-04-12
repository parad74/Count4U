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
using Count4U.Common.Behaviours;
using Count4U.Common.Constants;
using Count4U.Common.Helpers;
using Count4U.Model.Count4U;
using Count4U.Model.Interface.Audit;
using Count4U.Modules.Audit.ViewModels;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.Audit.Views
{
    /// <summary>
    /// Interaction logic for LocationAddEditDeleteView.xaml
    /// </summary>
    public partial class LocationAddEditDeleteView : UserControl, INavigationAware, IRegionMemberLifetime 
    {
        private readonly string backForwardRegionName;
        private readonly string searchFilterRegionName;

        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IContextCBIRepository _contextCbiRepository;
        private readonly Guid _guid;

        public LocationAddEditDeleteView(
            LocationAddEditDeleteViewModel viewModel,
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IContextCBIRepository contextCbiRepository)
        {
            this.InitializeComponent();

            this._contextCbiRepository = contextCbiRepository;
            this._eventAggregator = eventAggregator;

            this._regionManager = regionManager;
            this.DataContext = viewModel;

            this.dataGrid.SelectionChanged += listView_SelectionChanged;
            this.Loaded += LocationAddEditDeleteView_Loaded;

            System.Windows.Interactivity.Interaction.GetBehaviors(btnReports).Add(new ContextMenuLeftButtonBehavior());

            _guid = Guid.NewGuid();
            backForwardRegionName = Common.RegionNames.LocationAddEditDeleteBackForward + _guid.ToString();
            searchFilterRegionName = Common.RegionNames.LocationAddEditDeleteSearchFilter + _guid.ToString();

            viewModel.SearchFilterRegionKey = _guid.ToString();
        }

        public bool KeepAlive { get { return false; } }

        void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LocationAddEditDeleteViewModel viewModel = this.DataContext as LocationAddEditDeleteViewModel;
            if (viewModel != null)
                viewModel.SelectedItemsSet(dataGrid.SelectedItems.Cast<LocationItemViewModel>().ToList());
        }

        void LocationAddEditDeleteView_Loaded(object sender, RoutedEventArgs e)
        {
            LocationAddEditDeleteViewModel viewModel = this.DataContext as LocationAddEditDeleteViewModel;
            if (viewModel != null)
                viewModel.ReportButtonViewModel.BuildMenu(btnReports.ContextMenu);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            RegionManager.SetRegionName(backForward, backForwardRegionName);
            RegionManager.SetRegionName(searchFilter, searchFilterRegionName);

            Utils.SetCurrentAuditConfig(navigationContext, this._contextCbiRepository);
            Utils.MainWindowTitleSet(WindowTitles.LocationAddEditDelete, this._eventAggregator);

            UtilsNavigate.ApplicationStripNavigateFromNavigationContext(navigationContext, this._regionManager);
            UtilsNavigate.BackForwardNavigate(this._regionManager, backForwardRegionName);
            UtilsNavigate.SearchFilterNavigate(this._regionManager, searchFilterRegionName);
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
                                                      searchFilter
                                                  }, navigationContext);

            this._regionManager.Regions.Remove(backForwardRegionName);
            this._regionManager.Regions.Remove(searchFilterRegionName);
        }
    }
}
