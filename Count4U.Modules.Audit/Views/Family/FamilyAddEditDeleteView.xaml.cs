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
using Count4U.Model.Interface.Audit;
using Count4U.Modules.Audit.ViewModels.Family;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;

namespace Count4U.Modules.Audit.Views.Family
{
    
    public partial class FamilyAddEditDeleteView : UserControl, INavigationAware, IRegionMemberLifetime
    {
        private readonly IContextCBIRepository _contextCbiRepository;
        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;

        private readonly Guid _guid;
        private readonly string backForwardRegionName;
        private readonly string searchFilterRegionName;

		public FamilyAddEditDeleteView(
			FamilyAddEditDeleteViewModel viewModel,
            IContextCBIRepository contextCbiRepository,
            IEventAggregator eventAggregator,
            IRegionManager regionManager)
        {
            InitializeComponent();

            this.DataContext = viewModel;
            this._eventAggregator = eventAggregator;
            this._contextCbiRepository = contextCbiRepository;
            this._regionManager = regionManager;

			Loaded += FamilyAddEditDeleteView_Loaded;

            System.Windows.Interactivity.Interaction.GetBehaviors(btnReports).Add(new ContextMenuLeftButtonBehavior());

            _guid = Guid.NewGuid();

            viewModel.SearchFilterRegionKey = _guid.ToString();

			backForwardRegionName = Common.RegionNames.FamilyAddEditDeleteBackForward + _guid.ToString();
			searchFilterRegionName = Common.RegionNames.FamilyAddEditDeleteSearchFilter + _guid.ToString();

            dataGrid.SelectionChanged += dataGrid_SelectionChanged;
        }

        void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
			FamilyAddEditDeleteViewModel viewModel = this.DataContext as FamilyAddEditDeleteViewModel;
            if (viewModel == null) return;

			List<FamilyItemViewModel> items = new List<FamilyItemViewModel>();

			foreach (var item in dataGrid.SelectedItems.OfType<FamilyItemViewModel>())
            {
                items.Add(item);
            }

            viewModel.SetSelected(items);
        }

		void FamilyAddEditDeleteView_Loaded(object sender, RoutedEventArgs e)
        {
			FamilyAddEditDeleteViewModel viewModel = this.DataContext as FamilyAddEditDeleteViewModel;
            if (viewModel != null)
                viewModel.ReportButtonViewModel.BuildMenu(btnReports.ContextMenu);
        }

        public bool KeepAlive { get { return false; } }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            RegionManager.SetRegionName(backForward, backForwardRegionName);
            RegionManager.SetRegionName(searchFilter, searchFilterRegionName);

            Utils.SetCurrentAuditConfig(navigationContext, this._contextCbiRepository);

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
