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
using Count4U.Common.Constants;
using Count4U.Common.Helpers;
using Count4U.Model.Interface.Audit;
using Count4U.Modules.Audit.ViewModels.Catalog;
using Count4U.Modules.Audit.ViewModels.DevicePDA;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using System.Windows.Forms.Integration;
using Microsoft.Reporting.WinForms;

namespace Count4U.Modules.Audit.Views.Device
{
	public partial class DeviceFormView : UserControl, INavigationAware, IRegionMemberLifetime 
    {
        private readonly string backForwardRegionName;
        private readonly string searchFilterRegionName;

        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IContextCBIRepository _contextCbiRepository;
        private readonly Guid _guid;

		public DeviceFormView(
			DeviceFormViewModel viewModel, 
            IRegionManager regionManager, 
            IEventAggregator eventAggregator,
            IContextCBIRepository contextCbiRepository)
        {
            InitializeComponent();

            this._contextCbiRepository = contextCbiRepository;
            this._eventAggregator = eventAggregator;
            this._regionManager = regionManager;

            this.DataContext = viewModel;

            this.masterDataGrid.SelectionChanged += listView_SelectionChanged;
            this.Loaded += CatalogFormView_Loaded;

            _guid = Guid.NewGuid();
            backForwardRegionName = Common.RegionNames.CatalogFormBackForward+ _guid.ToString();
            searchFilterRegionName = Common.RegionNames.ProductSearchFilter + _guid.ToString();

            viewModel.SearchFilterRegionKey = _guid.ToString();
        }

        public bool KeepAlive { get { return false; } }

        void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
			DeviceFormViewModel viewModel = this.DataContext as DeviceFormViewModel;
            if (viewModel != null)
				viewModel.SelectedItemsSet(masterDataGrid.SelectedItems.Cast<DeviceItemViewModel>().ToList());
        }

        void CatalogFormView_Loaded(object sender, RoutedEventArgs e)
        {
			DeviceFormViewModel viewModel = this.DataContext as DeviceFormViewModel;
            if (viewModel != null)
                viewModel.ReportButton.BuildMenu(btnReport.ContextMenu);

			//wfhReport = new WindowsFormsHost();
			
			//wfhReport.Height = 200;
			//wfhReport.Width = 200;
			//rptViewer = new ReportViewer();
			//rptViewer.Height = 100;
			//rptViewer.Width = 100;
			//wfhReport.Child = rptViewer;
			
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            RegionManager.SetRegionName(backForward, backForwardRegionName);
            RegionManager.SetRegionName(searchFilter, searchFilterRegionName);

            Utils.SetCurrentAuditConfig(navigationContext, this._contextCbiRepository);
			Utils.MainWindowTitleSet(WindowTitles.Device, this._eventAggregator);

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
